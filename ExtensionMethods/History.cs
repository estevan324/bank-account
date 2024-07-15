using BankAccount.DTOs.Account;
using BankAccount.Entities;
using BankAccount.Entities.Enums;

namespace BankAccount.ExtensionMethods;

public static class History
{

    public static TransferResponse ConvertToTransferResponse(this HistoryDetail history)
    {
        return new TransferResponse(
            history.Date,
            Enum.GetName<TransactionType>(history.Type) ?? string.Empty,
            history.Amount,
            history.OriginId,
            history.DestinyId);
    }

    public static IEnumerable<TransferResponse> ConvertToTransferResponse(this IEnumerable<TransactionHistory> histories)
    {
        var newHistories = histories.Select(h => new TransferResponse(
            h.Date,
            Enum.GetName<TransactionType>(h.Type) ?? string.Empty,
            h.Amount,
            h.OriginId,
            h.DestinyId));

        return newHistories;
    }
}
