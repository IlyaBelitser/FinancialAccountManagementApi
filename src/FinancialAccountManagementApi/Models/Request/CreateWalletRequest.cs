using FluentValidation;

namespace FinancialAccountManagementApi.Models.Request;

public class CreateWalletRequest
{
    public required string UserId { get; init; }
    public required string Currency { get; init; }
    
    public class Validator : AbstractValidator<CreateWalletRequest>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Currency).IsEnumName(typeof(CurrencyCode), caseSensitive: false);
        }
    }
}