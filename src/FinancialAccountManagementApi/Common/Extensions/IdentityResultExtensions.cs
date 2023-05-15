using Ardalis.Result;
using Microsoft.AspNetCore.Identity;

namespace FinancialAccountManagementApi.Common.Extensions;

public static class IdentityResultExtensions
{
    public static List<ValidationError> GetErrors(this IdentityResult result)
    {
        return (List<ValidationError>) result.Errors.Select(e => new ValidationError
        {
            ErrorCode = e.Code,
            ErrorMessage = e.Description, 
        });
    }
}