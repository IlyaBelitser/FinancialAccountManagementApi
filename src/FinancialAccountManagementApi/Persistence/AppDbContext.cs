using FinancialAccountManagementApi.Models;
using FinancialAccountManagementApi.Models.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccountManagementApi.Persistence;

public sealed class AppDbContext : IdentityDbContext
{
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public new DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Wallet>()
            .Property(w => w.Currency)
            .HasConversion<string>();
    }
}