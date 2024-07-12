namespace BankAccount.DTOs.Auth;

public record TokenResponse(string Token, DateTime Expiration);