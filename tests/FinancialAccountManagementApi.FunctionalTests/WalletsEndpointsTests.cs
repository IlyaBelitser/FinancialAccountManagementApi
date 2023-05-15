using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bogus;
using FinancialAccountManagementApi.Common.Extensions;
using FinancialAccountManagementApi.Configurations;
using FinancialAccountManagementApi.Models;
using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Models.Authentication.Response;
using FinancialAccountManagementApi.Models.Response;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FinancialAccountManagementApi.FunctionalTests;

public class WalletsEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly SeedCredentialsSettings _seedCredentialsSettings;
    private readonly int _userCount = 50;
    private readonly int _threadCount = 10;
    private readonly int _transactionsCount = 10;

    public WalletsEndpointsTests(CustomWebApplicationFactory<Program> webAppFactory)
    {
        _client = webAppFactory.CreateDefaultClient();
        _seedCredentialsSettings = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()
            .GetSection(SeedCredentialsSettings.SectionName).Get<SeedCredentialsSettings>() ?? throw new InvalidOperationException();
    }

    [Fact]
    public async Task WalletsEndpoints_ChangingBalances_AllTransactionsHavePassed()
    {
        // Arrange
        Randomizer.Seed = new Random(8675309);
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        await SetJwtToAuthenticationHeaderAsync();
        IEnumerable<KeyValuePair<int, ICollection<decimal>>> transactionsData = await GetTransactionsDataAsync();

        // Act
        await ExecuteTransactionsAsync(transactionsData);
        
        // Assert
        await VerifyTransactionsAsync(transactionsData);
    }

    private async ValueTask SetJwtToAuthenticationHeaderAsync()
    {
        TokenDto token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Jwt);
    }

    private async ValueTask<TokenDto> GetAdminTokenAsync()
    {
        var request = (
            Url: "/api/auth",
            Body: new
            {
                UserName = _seedCredentialsSettings.Admin.UserName,
                Password = _seedCredentialsSettings.Admin.Password
            });

        HttpResponseMessage postResponse = await _client.PostAsync(request.Url, request.Body.ToStringContent());
        TokenDto tokenDto = await postResponse.Content.ReadFromJsonAsync<TokenDto>() ??
                            throw new InvalidOperationException(await postResponse.Content.ReadAsStringAsync());
        return tokenDto;
    }

    private async Task<IEnumerable<KeyValuePair<int, ICollection<decimal>>>> GetTransactionsDataAsync()
    {
        ICollection<int> walletIds = await CreateWalletIdsAsync();
        IEnumerable<KeyValuePair<int, ICollection<decimal>>> transactionsData = GenerateTransactionsData(walletIds);
        return transactionsData;
    }
    
    private async ValueTask<ICollection<int>> CreateWalletIdsAsync()
    {
        ICollection<User> users = GenerateUsers();
        ICollection<string> userIds = await CreateUserAccountAsync(users);
        await AssignRoleAsync(userIds);
        ICollection<int> wallets = await CreateWalletIdsAsync(userIds);

        return wallets;
    }
    
    private ICollection<User> GenerateUsers()
    {
        DateOnly dateOfBirthMinValue = DateOnly.FromDateTime(DateTime.Today).AddYears(-200);
        DateOnly dateOfBirthMaxValue = DateOnly.FromDateTime(DateTime.Today);

        var testUsers = new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.Patronymic, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.DateOfBirth, f => f.Date.BetweenDateOnly(dateOfBirthMinValue, dateOfBirthMaxValue));
        
        return testUsers.GenerateLazy(_userCount).ToList();
    }

    private async ValueTask<ICollection<string>> CreateUserAccountAsync(ICollection<User> users)
    {
        var userIds = new List<string>(users.Count);

        await Parallel.ForEachAsync(users, async (user, cancellationToken) =>
        {
            var request = (
                Url: "/api/users",
                Body: new
                {
                    UserName = $"{user.FirstName}{user.LastName}".Replace("'", ""),
                    FirstName = user.FirstName,
                    Patronymic = user.Patronymic,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Password = _seedCredentialsSettings.User.Password,
                });

            HttpResponseMessage postResponse = await _client.PostAsync(request.Url, request.Body.ToStringContent(), cancellationToken);
            if (postResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new InvalidOperationException($"""
                           StatusCode: {postResponse.StatusCode} 
                           User: {user.FirstName} {user.Patronymic} {user.LastName} {user.DateOfBirth}
                           HttpContent: {postResponse.Content.ReadAsStringAsync(cancellationToken).Result}
                           """);
            }

            UserDto userDto = await postResponse.Content.ReadFromJsonAsync<UserDto>(cancellationToken: cancellationToken) ??
                              throw new InvalidOperationException(postResponse.Content.ReadAsStringAsync(cancellationToken).Result);
            userIds.Add(userDto.Id);
        });

        return userIds;
    }

    private async ValueTask AssignRoleAsync(IEnumerable<string> userIds)
    {
        await Parallel.ForEachAsync(userIds, async (userId, cancellationToken) =>
        {
            var request = (
                Url: $"/api/users/{userId}/roles",
                Body: new
                {
                    Roles = new List<string> { Role.Basic }
                });

            HttpResponseMessage postResponse = await _client.PutAsync(request.Url, request.Body.ToStringContent(), cancellationToken);
            if (postResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"""
                           StatusCode: {postResponse.StatusCode} 
                           UserId: {userId}
                           HttpContent: {postResponse.Content.ReadAsStringAsync(cancellationToken).Result}
                           """);
            }
        });
    }

    private async ValueTask<ICollection<int>> CreateWalletIdsAsync(ICollection<string> userIds)
    {
        var walletIds = new List<int>(userIds.Count);
        
        await Parallel.ForEachAsync(userIds, async (userId, cancellationToken) =>
        {
            var request = (
                Url: "/api/wallets",
                Body: new
                {
                    UserId = userId,
                    Currency = CurrencyCode.RUB
                });

            HttpResponseMessage postResponse = await _client.PostAsync(request.Url, request.Body.ToStringContent(), cancellationToken);
            if (postResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new InvalidOperationException($"""
                           StatusCode: {postResponse.StatusCode} 
                           UserId: {userId}
                           HttpContent: {postResponse.Content.ReadAsStringAsync(cancellationToken).Result}
                           """);
            }

            WalletDto walletDto = await postResponse.Content.ReadFromJsonAsync<WalletDto>(cancellationToken: cancellationToken) ??
                                  throw new InvalidOperationException(postResponse.Content.ReadAsStringAsync(cancellationToken).Result);
            walletIds.Add(walletDto.Id);
        });
        
        return walletIds;
    }

    private IEnumerable<KeyValuePair<int, ICollection<decimal>>> GenerateTransactionsData(ICollection<int> walletIds)
    {
        var transactionsData = new Dictionary<int, ICollection<decimal>>(walletIds.Count);
        var faker = new Faker();
        
        foreach (int walletId in walletIds)
        {
            ICollection<decimal> transactions =
                Enumerable.Range(1, _transactionsCount)
                    .Select(_ => faker.Finance.Amount(min: -1000, max: 1000, decimals: 2)).ToList();
            transactionsData.Add(walletId, transactions);
        }
        
        return transactionsData;
    }

    private async ValueTask ExecuteTransactionsAsync(IEnumerable<KeyValuePair<int, ICollection<decimal>>> transactionsData)
    {
        ThreadPool.SetMinThreads(_threadCount, _threadCount);
         
         ParallelOptions parallelOptions = new()
         {
             MaxDegreeOfParallelism = _threadCount
         };
         
        await Parallel.ForEachAsync(transactionsData, async (transactionData, _) =>
        {
            await Parallel.ForEachAsync(transactionData.Value, parallelOptions, async (amount, cancellationToken) =>
            {
                var request = new { Url = $"/api/wallets/{transactionData.Key}?amount={amount}" };

                HttpResponseMessage putResponse = await _client.PatchAsync(request.Url, null, cancellationToken);
                if (putResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException($"""
                            StatusCode: {putResponse.StatusCode} 
                            WalletId: {transactionData.Key}
                            Amount: {amount}
                            HttpContent: {putResponse.Content.ReadAsStringAsync(cancellationToken).Result}
                            """);
                }
            });
        });
    }

    private async ValueTask VerifyTransactionsAsync(IEnumerable<KeyValuePair<int, ICollection<decimal>>> transactionsData)
    {
        await Parallel.ForEachAsync(transactionsData, async (transactionData, cancellationToken) =>
        {
            var request = new { Url = $"/api/wallets/{transactionData.Key}" };

            HttpResponseMessage response = await _client.GetAsync(request.Url, cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"""
                           StatusCode: {response.StatusCode} 
                           WalletId: {transactionData.Key}
                           HttpContent: {response.Content.ReadAsStringAsync(cancellationToken).Result}
                           """);
            }

            WalletDto walletDto = await response.Content.ReadFromJsonAsync<WalletDto>(cancellationToken: cancellationToken) ??
                                  throw new InvalidOperationException(response.Content.ReadAsStringAsync(cancellationToken).Result);
            walletDto.Balance.Should().Be(transactionData.Value.Sum());
        });
    }
}