using FluentValidation;

namespace FinancialAccountManagementApi.Models.Authentication.Request;

public class CreateUserRequest
{
    public required string UserName { get; init; }
    public string? FirstName { get; init; }
    public string? Patronymic { get; init; }
    public string? LastName { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string Password { get; init; }
    
    public class Validator : AbstractValidator<CreateUserRequest>
    {
        readonly DateOnly _dateOfBirthMinValue = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-200);
        readonly DateOnly _dateOfBirthMaxValue = DateOnly.FromDateTime(DateTime.UtcNow);
        
        public Validator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotNull().InclusiveBetween(_dateOfBirthMinValue, _dateOfBirthMaxValue);
            RuleFor(x => x.Password).NotNull();
        }
    }
}