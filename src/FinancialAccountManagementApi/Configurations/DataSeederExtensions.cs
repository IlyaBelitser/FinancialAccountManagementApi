using FinancialAccountManagementApi.Persistence.Initialization;
using FinancialAccountManagementApi.Common.Validations;
using IServiceScope = Microsoft.Extensions.DependencyInjection.IServiceScope;

namespace FinancialAccountManagementApi.Configurations;

public static class DataSeederExtensions
{
    public static IServiceCollection AddDataSeeder(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SeedCredentialsSettings>(configuration.GetRequiredSection(SeedCredentialsSettings.SectionName));
        services.AddOptions<SeedCredentialsSettings>()
            .BindConfiguration(SeedCredentialsSettings.SectionName)
            .ValidateFluently();
        
        services.AddScoped<IDataSeeder, IdentityDataSeeder>();

        return services;
    }
    public static async Task<IApplicationBuilder> UseDataSeeder(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        foreach (IDataSeeder seeder in scope.ServiceProvider.GetServices<IDataSeeder>())
        {
            await seeder.SeedAllAsync();
        }
        
        return app;
    }
}