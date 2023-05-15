using FinancialAccountManagementApi.Common.Attributes;
using FluentValidation;

namespace FinancialAccountManagementApi.Configurations;

[Option]
public class JwtSettings
{
    public static readonly string SectionName = "Jwt";
    
    public required string Key { get; init; }
    public int TokenExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationInDays { get; init; }
    
    public class Validator : AbstractValidator<JwtSettings>
    {
        public Validator()
        {
            RuleFor(x => x.Key).NotEmpty();
            RuleFor(x => x.TokenExpirationInMinutes).NotEmpty().GreaterThanOrEqualTo(0);
            RuleFor(x => x.RefreshTokenExpirationInDays).NotEmpty().GreaterThanOrEqualTo(0);
        }
    }    
}