using BankAccount.Entities.Enums;

namespace BankAccount.DTOs.Account;

public record HistoryDetail(double Amount, DateTime Date, TransactionType Type, Guid OriginId, Guid DestinyId);
