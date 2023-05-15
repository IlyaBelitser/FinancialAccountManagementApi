using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;

namespace FinancialAccountManagementApi.Services.Authentication;

public class RoleService : IRoleService
{    
    private readonly UserManager<User> _userManager;

    public RoleService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IList<string>>> GetAsync(string userId)
    {
        User? user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.NotFound($"User [{userId}] Not Found");
        }
        
        return await GetAsync(user);
    }
    
    public async Task<Result<IList<string>>> GetAsync(User user)
    {
        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        return Result.Success(userRoles);
    }

    public async Task<Result> UpdateAsync(string userId, ICollection<string> roles, CancellationToken cancellationToken = default)
    {
        User? user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return Result.NotFound($"User [{userId}] Not Found");
        }

        return await UpdateAsync(user, roles);
    }

    public async Task<Result> UpdateAsync(User user, ICollection<string> roles)
    {
        IEnumerable<string> userRoles = await _userManager.GetRolesAsync(user);
        IEnumerable<string> addedRoles = roles.Except(userRoles);
        IEnumerable<string> removedRoles = userRoles.Except(roles);
 
        IdentityResult result = await _userManager.AddToRolesAsync(user, addedRoles);
        if (!result.Succeeded)
        {
            return Result.Invalid(result.GetErrors());
        }
 
        result = await _userManager.RemoveFromRolesAsync(user, removedRoles);
        
        return result.Succeeded ? Result.Success() : Result.Invalid(result.GetErrors());
    }
}