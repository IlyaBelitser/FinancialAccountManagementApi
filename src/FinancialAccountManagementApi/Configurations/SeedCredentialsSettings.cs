using FinancialAccountManagementApi.Common.Attributes;
using FluentValidation;

namespace FinancialAccountManagementApi.Configurations;

[Option]
public class SeedCredentialsSettings
{
    public static readonly string SectionName = "SeedCredentials";
    
    public required UserSettings Admin { get; init; }
    public required UserSettings User { get; init; }
    
    public class Validator : AbstractValidator<SeedCredentialsSettings>
    {
        public Validator()
        {
            RuleFor(x => x.Admin).SetValidator(new UserSettings.Validator());
            RuleFor(x => x.User).SetValidator(new UserSettings.Validator());
        }
    }
}

[Option]
public class UserSettings
{
    public required string UserName { get; init; }
    public required string Password { get; init; }
    
    public class Validator : AbstractValidator<UserSettings>
    {
        public Validator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotNull();
        }
    }
}