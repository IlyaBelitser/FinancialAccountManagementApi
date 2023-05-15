using FluentValidation;

namespace FinancialAccountManagementApi.Models.Authentication.Request;

public class CreateTokenRequest
{
    public required string UserName { get; init; }
    public required string Password { get; init; }

    public class Validator : AbstractValidator<CreateTokenRequest>
    {
        public Validator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotNull();
        }
    }
}