using FinancialAccountManagementApi.Models.Authentication;
using FinancialAccountManagementApi.Models.Authentication.Response;
using Mapster;

namespace FinancialAccountManagementApi.Configurations;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<User, UserDto>.NewConfig()
            .Map(dest => dest.Roles,
                src => MapContext.Current!.Parameters["roles"]);
    }
}