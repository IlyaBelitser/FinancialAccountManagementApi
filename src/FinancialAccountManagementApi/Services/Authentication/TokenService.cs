using System.Security.Claims;
using FinancialAccountManagementApi.Common.Factories;
using FinancialAccountManagementApi.Configurations;
using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Common.Extensions;
using FinancialAccountManagementApi.Models.Authentication.Response;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ardalis.Result;

namespace FinancialAccountManagementApi.Services.Authentication;

public class TokenService : ITokenService
{    
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly TokenFactory _tokenFactory;

    public TokenService(UserManager<User> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _tokenFactory = new TokenFactory(_jwtSettings);
    }

    public async Task<Result<TokenDto>> GetTokenAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.Users.Include(u => u.Token)
                .SingleOrDefaultAsync(u => u.UserName == userName, cancellationToken) is not { } user)
        {
            return Result.Error("User not found");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Result.Error("Password is not valid");
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        user.Token = _tokenFactory.CreateToken(user, roles);
        
        IdentityResult result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result.Invalid(result.GetErrors());
        }

        return Result.Success(user.Token.Adapt<TokenDto>());
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(string jwt, string refreshToken, CancellationToken cancellationToken = default)
    {
        ClaimsPrincipal? userPrincipal = ClaimsPrincipalFactory.CreateClaimsPrincipal(jwt, _jwtSettings.Key);
        if (userPrincipal is null)
        {
            return Result.Error("Invalid JWT");
        }
        
        string? userId = userPrincipal.GetUserId();

        if (await _userManager.Users.Include(u => u.Token)
                .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken) is not { } user)
        {
            return Result.Error("Invalid JWT");
        }

        if (user.Token is null)
        {
            return Result.NotFound("Refresh Token Not Found");
        }

        if (user.Token.RefreshToken != refreshToken)
        {
            return Result.Error("Invalid Refresh Token");
        }

        if (user.Token.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Error("Refresh Token Expired");
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        user.Token = _tokenFactory.CreateToken(user, roles);
        
        IdentityResult result = await _userManager.UpdateAsync(user);
        
        return result.Succeeded ? Result.Success(user.Token.Adapt<TokenDto>()) : Result.Invalid(result.GetErrors());
    }
}