using BankAccount.DTOs.Clients;
using BankAccount.Entities;
using BankAccount.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankAccount.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    public AccountController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAccount([FromBody] ClientRegistrationDTO client)
    {
        var userDto = new UserDTO(client.Name!, client.Cpf!, client.Password!);
        var userCreated = await _unitOfWork.CustomerRepository.CreateAsync(userDto);

        var account = _unitOfWork.AccountRepository.Create(new Account(client.InitialBalance, userCreated.Id));
        await _unitOfWork.CommitAsync();

        return account.Id;
    }
}
