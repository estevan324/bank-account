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
    public DateTime Date { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Required]
    public Guid OriginId { get; set; }

    [Required]
    public Guid DestinyId { get; set; }

    public Account? Origin { get; set; }
    public Account? Destiny { get; set; }

    public TransactionHistory(double amount, DateTime date, TransactionType type, Guid originId, Guid destinyId)
    {
        Id = new Guid();
        Amount = amount;
        Date = date;
        Type = type;
        OriginId = originId;
        DestinyId = destinyId;
    }
}
