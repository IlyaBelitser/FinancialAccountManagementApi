using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Persistence;
using Microsoft.AspNetCore.Identity;

namespace FinancialAccountManagementApi.Configurations;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.SignIn.RequireConfirmedAccount = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._ ";
            })
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddJwtAuth(configuration);

        return services;
    }
}