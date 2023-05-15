using FinancialAccountManagementApi.Persistence;
using FinancialAccountManagementApi.Persistence.Initialization;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccountManagementApi.Configurations;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = "DataSource = " + AppDomain.CurrentDomain.BaseDirectory +
                               configuration.GetConnectionString("DefaultFilename");

        services.AddDbContext<AppDbContext>(
            dbContextOptions => dbContextOptions
                .UseSqlite(
                    connectionString
                )
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );
        services.AddScoped<IDataSeeder, IdentityDataSeeder>();

        return services;
    }
}