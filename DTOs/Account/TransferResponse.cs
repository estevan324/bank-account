namespace BankAccount.DTOs.Account;

public record TransferResponse(DateTime Date, string Type, double Amount, Guid OriginId, Guid DestinyId);
