using FinancialAccountManagementApi.Common.Attributes;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace FinancialAccountManagementApi.Configurations;

public static class FluentValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        AssemblyScanner.FindValidatorsInAssemblyContaining<Program>().ForEach(pair =>
        {
            services.Add(pair.ValidatorType.IsDefined(typeof(OptionAttribute), false)
                ? ServiceDescriptor.Singleton(pair.InterfaceType, pair.ValidatorType)
                : ServiceDescriptor.Transient(pair.InterfaceType, pair.ValidatorType));
        });

        services.AddFluentValidationAutoValidation();
        
        return services;
    }
}