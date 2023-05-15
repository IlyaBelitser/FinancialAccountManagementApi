using FinancialAccountManagementApi.Configurations;
using FinancialAccountManagementApi.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FinancialAccountManagementApi.Persistence.Initialization;

public class IdentityDataSeeder : IDataSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly SeedCredentialsSettings _seedCredentialsSettings;

    public IdentityDataSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, 
        IOptions<SeedCredentialsSettings> seedCredentialsSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _seedCredentialsSettings = seedCredentialsSettings.Value;
    }

    public async Task SeedAllAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    private async Task SeedRoles()
    {
        if (!await _roleManager.RoleExistsAsync(Role.Admin))
            await _roleManager.CreateAsync(new IdentityRole(Role.Admin));

        if (!await _roleManager.RoleExistsAsync(Role.Basic))
            await _roleManager.CreateAsync(new IdentityRole(Role.Basic));
    }

    private async Task SeedUsers()
    {
        if (await _userManager.FindByNameAsync(_seedCredentialsSettings.Admin.UserName) == null)
        {
            var user = new User { UserName = _seedCredentialsSettings.Admin.UserName };

            IdentityResult result = await _userManager.CreateAsync(user, _seedCredentialsSettings.Admin.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Role.Admin);
            }
        }

        if (await _userManager.FindByNameAsync(_seedCredentialsSettings.User.UserName) == null)
        {
            var user = new User { UserName = _seedCredentialsSettings.User.UserName };

            IdentityResult result = await _userManager.CreateAsync(user, _seedCredentialsSettings.User.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Role.Basic);
            }
        }
    }
}
