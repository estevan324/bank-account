using BankAccount.DTOs.Clients;
using BankAccount.Entities;
using BankAccount.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<Guid>> CreateAccount([FromBody] ClientRegisterRequest client)
    {
        var userDto = new UserDetail(client.Name!, client.Cpf!, client.Password!);
        var userCreated = await _unitOfWork.CustomerRepository.CreateAsync(userDto);

        var account = _unitOfWork.AccountRepository.Create(new Account(client.InitialBalance, userCreated.Id));
        await _unitOfWork.CommitAsync();

        return account.Id;
    }

    [Authorize]
    [HttpGet("balance")]
    public async Task<ActionResult> ShowBalance([FromQuery] Guid accountId)
    {
        var balance = await _unitOfWork.AccountRepository.ShowBalance(accountId);
        if (balance is null)
            return NotFound("Account not found");

        return Ok(new { Balance = balance });
    }
}
