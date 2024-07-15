using BankAccount.DTOs.Account;
using BankAccount.Entities.Enums;

namespace BankAccount.ExtensionMethods
{
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
    }
}
