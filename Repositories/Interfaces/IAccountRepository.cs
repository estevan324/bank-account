using BankAccount.Entities;

namespace BankAccount.Repositories.Interfaces;

public interface IAccountRepository
{
    Account Create(Account account);
}
