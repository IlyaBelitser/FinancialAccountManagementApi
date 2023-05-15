using Ardalis.Result;
using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Common.Extensions;
using FinancialAccountManagementApi.Models.Authentication.Response;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccountManagementApi.Services.Authentication;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<Result<IList<UserDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<User> users = await _userManager.Users.AsNoTracking().Include(u => u.Wallet)
            .ToListAsync(cancellationToken: cancellationToken);
        
        var userDtoList = new List<UserDto>();
        foreach (User user in users)
        {
            UserDto userDto = user.BuildAdapter()
                .AddParameters("roles", await _userManager.GetRolesAsync(user))
                .AdaptToType<UserDto>();
            userDtoList.Add(userDto);
        }

        return Result.Success((IList<UserDto>) userDtoList);
    }
    
    public async Task<Result<UserDto>> CreateAsync(UserDto userDto, string password)
    {
        User? user = await _userManager.FindByNameAsync(userDto.UserName);
        if (user is not null)
        {
            return Result.Conflict("User is already exist");
        }
        
        user = userDto.Adapt<User>();

        IdentityResult result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return Result.Invalid(result.GetErrors());
        }

        return Result.Success(user.BuildAdapter()
            .AddParameters("roles", new List<string>())
            .AdaptToType<UserDto>(),
            $"/api/users/{user.Id}");
    }
    
    public async Task<Result<UserDto>> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        User? user = await _userManager.Users.AsNoTracking().Include(u => u.Wallet)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
        if (user is null)
        {
            return Result.NotFound($"User [{userId}] Not Found");
        }

        UserDto userDto = user.BuildAdapter()
            .AddParameters("roles", await _userManager.GetRolesAsync(user))
            .AdaptToType<UserDto>();

        return Result.Success(userDto);
    }
    
    public async Task<Result> DeleteAsync(string userId)
    {
        User? user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.NotFound($"User [{userId}] Not Found");
        }

        IdentityResult result = await _userManager.DeleteAsync(user);
        
        return result.Succeeded ? Result.Success() : Result.Invalid(result.GetErrors());
    }
}