using BankAccount.DTOs.Account;
using BankAccount.Entities;

namespace BankAccount.Repositories.Interfaces;

public interface IAccountRepository
{
    Account Create(Account account);
    Task<Account?> Get(Guid accountId);
    Account Update(Account account);
    void SaveHistory(HistoryDetail history);
}
