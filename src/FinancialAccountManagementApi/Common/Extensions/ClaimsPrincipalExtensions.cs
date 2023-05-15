using System.Security.Claims;

namespace FinancialAccountManagementApi.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
       => principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}