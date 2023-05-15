using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace FinancialAccountManagementApi.Models.Authentication.Request;

public class UpdateRolesRequest
{
    public required List<string> Roles { get; init; }
    
    public class Validator : AbstractValidator<UpdateRolesRequest>
    {
        public Validator(RoleManager<IdentityRole> roleManager)
        {
            RuleForEach(x => x.Roles).Custom(async (role, context) => 
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    context.AddFailure($"Role '{role}' does not exist.");
                }
            });
        }
    }
}