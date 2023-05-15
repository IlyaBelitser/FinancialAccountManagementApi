using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialAccountManagementApi.Models.Authentication;

[Table("Tokens")]
public class Token
{
    public int Id { get; init; }

    [NotMapped]
    public required string Jwt { get; init; }
    
    [Required]
    public required string RefreshToken { get; init; }
    
    public DateTime RefreshTokenExpiryTime { get; init; }
    
    [Required]
    public required string UserId { get; init; }
}