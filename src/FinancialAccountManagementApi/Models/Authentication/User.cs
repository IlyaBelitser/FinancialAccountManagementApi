using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancialAccountManagementApi.Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace FinancialAccountManagementApi.Models.Authentication;

public class User : IdentityUser, IAggregateRoot
{
    public string? FirstName { get; set; }
    public string? Patronymic { get; set; }
    public string? LastName { get; set; }
    
    [Required]
    [Column(TypeName = "date")]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    public DateTime CreateDate {get; set;}

    public Token? Token { get; set; }
    public Wallet? Wallet { get; set; }
}