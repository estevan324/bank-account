using BankAccount.Entities;
using BankAccount.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankAccount.Context;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<TransactionHistory> TransactionHistories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TransactionHistory>()
            .Property(t => t.Type)
            .HasMaxLength(20)
            .HasConversion(c => c.ToString(), c => Enum.Parse<TransactionType>(c));
    }
}
