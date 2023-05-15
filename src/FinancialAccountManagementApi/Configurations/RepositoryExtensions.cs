using FinancialAccountManagementApi.Common.Contracts;
using FinancialAccountManagementApi.Persistence;

namespace FinancialAccountManagementApi.Configurations;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        return services;
    }
}