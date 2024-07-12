using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankAccount.Entities;

public class Account
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public double Balance { get; set; }

    public string UserId { get; set; }

    [JsonIgnore]
    public IdentityUser? User { get; set; }

    public Account(double balance, string userId)
    {
        Id = new Guid();
        Balance = balance;
        UserId = userId;
    }
}
