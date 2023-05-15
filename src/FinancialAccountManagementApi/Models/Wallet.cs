using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancialAccountManagementApi.Common.Contracts;

namespace FinancialAccountManagementApi.Models;

public class Wallet : IAggregateRoot
{
    public int Id { get; init; }

    [Column(TypeName = "money")]
    [ConcurrencyCheck]
    public decimal Balance { get; set; }
    
    [Required]
    public CurrencyCode Currency { get; init; } = CurrencyCode.RUB;
    
    [Required]
    public DateTime CreateDate { get; init; }
    
    [Required]
    public required string UserId { get; init; }
}