﻿using System.ComponentModel.DataAnnotations;

namespace BankAccount.DTOs.Account
{
    public class TransferRequest
    {
        [Required]
        public Guid Origin { get; set; }

        [Required]
        public Guid Target { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public double Amount { get; set; }
    }
}
