using FinancialAccountManagementApi.Common.Contracts;
using FinancialAccountManagementApi.Models;
using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Common.Extensions;
using FinancialAccountManagementApi.Models.Response;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Ardalis.Result;

namespace FinancialAccountManagementApi.Services;

public class WalletService : IWalletService
{
    private readonly UserManager<User> _userManager;
    private readonly IRepository<Wallet> _repository;
    private readonly IReadRepository<Wallet> _readRepository;

    public WalletService(UserManager<User> userManager, IRepository<Wallet> repository, IReadRepository<Wallet> readRepository)
    {
        _userManager = userManager;
        _repository = repository;
        _readRepository = readRepository;
    }
    
    public async Task<Result<IList<WalletDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        List<Wallet> walletList = await _readRepository.ListAsync(cancellationToken);

        return Result.Success(walletList.Adapt<IList<WalletDto>>());
    }

    public async Task<Result<WalletDto>> CreateAsync(string userId, CurrencyCode currency, CancellationToken cancellationToken = default)
    {
        if (await _userManager.Users.Include(u => u.Wallet)
                .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken) is not { } user)
        {
            return Result.Error("User not found");
        }

        if (user.Wallet is not null)
        {
            return Result.Conflict("Wallet is already exist");
        }
        
        user.Wallet = new Wallet { Currency = currency, UserId = userId, CreateDate = DateTime.UtcNow };
        IdentityResult result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result.Invalid(result.GetErrors());
        }

        return Result.Success(user.Wallet.Adapt<WalletDto>(), $"/api/wallets/{user.Wallet.Id}");
    }

    public async Task<Result<WalletDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await _readRepository.GetByIdAsync(id, cancellationToken);
        if (wallet is null)
        {
            return Result.NotFound($"Wallet [{id}] Not Found");
        }

        return Result.Success(wallet.Adapt<WalletDto>());
    }

    public async Task<Result<WalletDto>> UpdateAsync(int id, decimal amount, CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await _repository.GetByIdAsync(id, cancellationToken);
        if (wallet is null)
        {
            return Result.NotFound($"Wallet [{id}] Not Found");
        }

        wallet.Balance += amount;

        bool isSaved = false;
        while (!isSaved)
        {
            try
            {
                await _repository.UpdateAsync(wallet, cancellationToken);
                isSaved = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                EntityEntry entry = ex.Entries.Single();
                PropertyValues? databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

                if (databaseValues is null)
                {
                    return Result.NotFound($"Wallet [{wallet.Id}] is deleted");
                }

                if (entry.Entity is Wallet)
                {
                    Wallet databaseEntity = (Wallet) databaseValues.ToObject();

                    entry.Property(nameof(Wallet.Balance)).OriginalValue = databaseEntity.Balance;
                    entry.Property(nameof(Wallet.Balance)).CurrentValue = databaseEntity.Balance + amount;
                }
                else
                {
                    throw new NotSupportedException(
                        "Don't know how to handle concurrency conflicts for "
                        + entry.Metadata.Name);
                }
            }
        }

        return Result.Success(wallet.Adapt<WalletDto>());
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        
        Wallet? wallet = await _readRepository.GetByIdAsync(id, cancellationToken);
        if (wallet is null)
        {
            return Result.NotFound($"Wallet [{id}] not found");
        }

        await _repository.DeleteAsync(wallet, cancellationToken);
        
        return Result.Success();
    }
}