using BankAccount.DTOs.Clients;
using Microsoft.AspNetCore.Identity;

namespace BankAccount.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<IdentityUser> CreateAsync(UserDetail userDto);
}
