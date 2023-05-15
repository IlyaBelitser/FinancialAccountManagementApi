using Ardalis.Result;
using FinancialAccountManagementApi.Common.Contracts;
using FinancialAccountManagementApi.Models;
using FinancialAccountManagementApi.Models.Response;

namespace FinancialAccountManagementApi.Services;

public interface IWalletService : ITransientService
{
    Task<Result<IList<WalletDto>>> GetListAsync(CancellationToken cancellationToken = default);

    Task<Result<WalletDto>> CreateAsync(string userId, CurrencyCode currency, CancellationToken cancellationToken = default);

    Task<Result<WalletDto>> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<WalletDto>> UpdateAsync(int id, decimal amount, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}