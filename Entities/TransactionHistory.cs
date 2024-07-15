using BankAccount.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccount.Entities;

public class TransactionHistory
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public double Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    public TransactionHistory(double amount, DateOnly date, TransactionType type)
    {
        Id = new Guid();
        Amount = amount;
        Date = date;
        Type = type;
    }
}
