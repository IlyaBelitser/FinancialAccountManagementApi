using System.Runtime.Serialization;
using Mapster;

namespace FinancialAccountManagementApi.Models.Authentication.Response;

[AdaptTo(nameof(User), IgnoreAttributes = new[] { typeof(IgnoreDataMemberAttribute) })]
public class UserDto
{
    [IgnoreDataMember]
    public required string Id { get; init; }

    public required string UserName { get; init; }
    public string? FirstName { get; init; }
    public string? Patronymic { get; init; }
    public string? LastName { get; init; }
    public DateOnly DateOfBirth { get; init; }
    public DateTime CreateDate { get; init; } = DateTime.UtcNow;
    
    [IgnoreDataMember]
    public int? WalletId { get; init; }

    public required IList<string> Roles { get; init; }
}