using BankAccount.Context;
using BankAccount.DTOs.Account;
using BankAccount.Entities;
using BankAccount.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BankAccount.Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;
    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Account Create(Account account)
    {
        _context.Accounts.Add(account);

        return account;
    }

    public async Task<Account?> GetAsync(Expression<Func<Account, bool>> predicate)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<TransactionHistory>> GetHistoriesAsync(int page, int pageLimit, Guid accountId)
    {
        return await _context.TransactionHistories
            .Where(t => t.OriginId == accountId || t.DestinyId == accountId)
            .Skip((page - 1) * pageLimit)
            .Take(pageLimit)
            .AsNoTracking()
            .ToListAsync();
    }

    public void SaveHistory(HistoryDetail history)
    {
        var transactionHistory = new TransactionHistory(history.Amount, history.Date, history.Type, history.OriginId, history.DestinyId);

        _context.TransactionHistories.Add(transactionHistory);
    }

    public Account Update(Account account)
    {
        _context.Accounts.Update(account);

        return account;
    }
}
