namespace BankAccount.Repositories.Interfaces;

public interface IUnitOfWork
{
    IAccountRepository AccountRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    Task CommitAsync();
    void Dispose();
}
