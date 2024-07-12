using BankAccount.Validation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BankAccount.DTOs.Auth;

public class LoginRequest
{
    [Required]
    [CPF]
    public string? Cpf { get; set; }

    [Required]
    public string? Password { get; set; }
}
