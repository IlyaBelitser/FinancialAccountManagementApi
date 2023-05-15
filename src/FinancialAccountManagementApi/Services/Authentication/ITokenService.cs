using Ardalis.Result;
using FinancialAccountManagementApi.Common.Contracts;
using FinancialAccountManagementApi.Models.Authentication.Response;

namespace FinancialAccountManagementApi.Services.Authentication;

public interface ITokenService : ITransientService
{
    Task<Result<TokenDto>> GetTokenAsync(string userName, string password, CancellationToken cancellationToken = default);

    Task<Result<TokenDto>> RefreshTokenAsync(string jwt, string refreshToken, CancellationToken cancellationToken = default);
}