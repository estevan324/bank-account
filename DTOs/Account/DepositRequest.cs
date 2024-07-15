using System.ComponentModel.DataAnnotations;

namespace BankAccount.DTOs.Account;

public class DepositRequest
{
    [Required]
    public Guid Target { get; set; }

    [Required]
    [Range(1, double.MaxValue)]
    public double Amount { get; set; }
}
