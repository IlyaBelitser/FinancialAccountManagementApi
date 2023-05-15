using FluentValidation;

namespace FinancialAccountManagementApi.Models.Authentication.Request;

public class RefreshTokenRequest
{
    public required string Jwt { get; init; }
    public required string RefreshToken { get; init; }

    public class Validator : AbstractValidator<RefreshTokenRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Jwt).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}