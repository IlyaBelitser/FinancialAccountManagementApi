using Ardalis.Result;
using FinancialAccountManagementApi.Common.Contracts;
using FinancialAccountManagementApi.Models.Authentication.Response;

namespace FinancialAccountManagementApi.Services.Authentication;

public interface IUserService : ITransientService
{
    Task<Result<IList<UserDto>>> GetListAsync(CancellationToken cancellationToken = default);

    Task<Result<UserDto>> CreateAsync(UserDto userDto, string password);
    
    Task<Result<UserDto>> GetAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(string userId);
}