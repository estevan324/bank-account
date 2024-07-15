using BankAccount.DTOs.Account;
using BankAccount.Entities;
using System.Linq.Expressions;

namespace BankAccount.Repositories.Interfaces;

public interface IAccountRepository
{
    Account Create(Account account);
    Task<Account?> GetAsync(Expression<Func<Account, bool>> predicate);
    Account Update(Account account);
    void SaveHistory(HistoryDetail history);
    Task<IEnumerable<TransactionHistory>> GetHistoriesAsync(int page, int pageLimit, Guid accountId);
}
