using FinancialAccountManagementApi.Common.Contracts;
using FinancialAccountManagementApi.Models.Authentication;
using Ardalis.Result;

namespace FinancialAccountManagementApi.Services.Authentication;

public interface IRoleService : ITransientService
{   
    Task<Result<IList<string>>> GetAsync(string userId);
    
    Task<Result<IList<string>>> GetAsync(User user);

    Task<Result> UpdateAsync(string userId, ICollection<string> roles, CancellationToken cancellationToken = default);
    
    Task<Result> UpdateAsync(User user, ICollection<string> roles);
}