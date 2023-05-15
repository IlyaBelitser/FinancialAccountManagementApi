using FinancialAccountManagementApi.Common.Middlewares;

namespace FinancialAccountManagementApi.Configurations;

public static class ExceptionMiddlewareExtensions
{
    public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
    {
        return services.AddScoped<ExceptionMiddleware>();
    }

    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}