using BankAccount.Context;
using BankAccount.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BankAccount.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly IAccountRepository? _accountRepository;
    private readonly ICustomerRepository? _customerRepository;

    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UnitOfWork(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IAccountRepository AccountRepository => _accountRepository ?? new AccountRepository(_context);

    public ICustomerRepository CustomerRepository => _customerRepository ?? new CustomerRepository(_userManager);

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
