using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace FinancialAccountManagementApi.Common.Validations;

/// <summary>
/// Implementation of <see cref="IValidateOptions{TOptions}"/> that uses FluentValidation's <see cref="Validator"/> for validation.
/// </summary>
/// <typeparam name="TOptions">The instance being validated.</typeparam>
public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
    private readonly IValidator<TOptions> _validator;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">The name of the option.</param>
    /// <param name="validator"></param>
    public FluentValidationOptions(string? name, IValidator<TOptions> validator)
    {
        Name = name;
        _validator = validator;
    }

    /// <summary>
    /// The options name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Validates a specific named options instance (or all when <paramref name="name"/> is null).
    /// </summary>
    /// <param name="name">The name of the options instance being validated.</param>
    /// <param name="options">The options instance.</param>
    /// <returns>The <see cref="ValidateOptionsResult"/> result.</returns>
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (Name != null && Name != name)
        {
            return ValidateOptionsResult.Skip;
        }

        ArgumentNullException.ThrowIfNull(options);
        
        var validationResults = _validator.Validate(options);
        if (validationResults.IsValid)
        {
            return ValidateOptionsResult.Success;    
        }

        var errors = validationResults.Errors.Select(x =>
            $"Options validation failed for '{x.PropertyName}' with error: '{x.ErrorMessage}'.");
        
        return ValidateOptionsResult.Fail(errors);
    }
}