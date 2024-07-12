using BankAccount.Validation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BankAccount.DTOs.Clients;

public class ClientRegisterRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "The name is required and must be between 1 and 200 characters")]
    public string? Name { get; set; }

    [Required]
    [CPF(ErrorMessage = "The CPF must have 11 characters")]
    public string? Cpf { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "The initial balance must be greater or equal to zero")]
    public double InitialBalance { get; set; }

}
