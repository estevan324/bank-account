namespace BankAccount.DTOs.Account
{
    public record TransferRequest(Guid Origin, Guid Target, double Amount);
}
