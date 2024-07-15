using System.ComponentModel.DataAnnotations;

namespace BankAccount.DTOs.Account;

public class WithdrawRequest
{
    [Required]
    [Range(1, double.MaxValue)]
    public double Amount { get; set; }
}
