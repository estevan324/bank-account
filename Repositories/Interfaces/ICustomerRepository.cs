using BankAccount.DTOs.Clients;

namespace BankAccount.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<Guid> CreateAsync(UserDTO userDto);
}
