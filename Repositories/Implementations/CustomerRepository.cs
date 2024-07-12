using BankAccount.DTOs.Clients;
using BankAccount.Exceptions;
using BankAccount.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BankAccount.Repositories.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly UserManager<IdentityUser> _userManager;
    public CustomerRepository(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityUser> CreateAsync(UserDTO userDto)
    {
        var user = new IdentityUser
        {
            UserName = userDto.Cpf
        };

        var claims = new List<Claim>
        {
            new("CPF", userDto.Cpf),
            new(ClaimTypes.Name, userDto.Name)
        };

        var result = await _userManager.CreateAsync(user, userDto.Password);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new UserCreationException($"User creation failed: {errorMessage}");
        }

        await _userManager.AddClaimsAsync(user, claims);

        return user;
    }
}
