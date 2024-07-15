﻿using BankAccount.Context;
using BankAccount.Entities;
using BankAccount.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Account?> Get(Guid accountId)
    {
        return await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == accountId);
    }
}
