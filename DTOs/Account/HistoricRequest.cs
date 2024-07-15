using System.ComponentModel.DataAnnotations;

namespace BankAccount.DTOs.Account;

public class HistoricRequest
{
    [Required]
    public int Page { get; set; } = 1;

    [Required]
    public int PageLimit { get; set; } = 10;
}
