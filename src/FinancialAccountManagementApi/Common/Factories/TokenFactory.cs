using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinancialAccountManagementApi.Configurations;
using FinancialAccountManagementApi.Models.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace FinancialAccountManagementApi.Common.Factories;

public class TokenFactory
{
    private readonly JwtSettings _jwtSettings;

    public TokenFactory(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }
    
    public Token CreateToken(User user, IList<string>? roles)
    {
        string jwt = GenerateJwt(user, roles);
        string refreshToken = GenerateRefreshToken();
        DateTime refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        return new Token
        {
            Jwt = jwt, 
            RefreshToken = refreshToken, 
            RefreshTokenExpiryTime = refreshTokenExpiryTime,
            UserId = user.Id
        };
    }

    private string GenerateJwt(User user, IList<string>? roles)
    {
        return GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, roles));
    }

    private static IEnumerable<Claim> GetClaims(User user, IList<string>? roles)
    {
        var result = new List<Claim>();
        result.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        
        if (roles != null)
        {
            result.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        return result;
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}