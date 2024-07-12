using BankAccount.DTOs.Clients;
using BankAccount.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankAccount.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    public AccountController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAccount([FromBody] ClientRegistrationDTO client)
    {
        var user = new UserDTO(client.Name!, client.Cpf!, client.Password!);
        var result = await _customerRepository.CreateAsync(user);

        return result;
    }
}
