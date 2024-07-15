using BankAccount.DTOs.Account;
using BankAccount.DTOs.Clients;
using BankAccount.Entities;
using BankAccount.Entities.Enums;
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
        var account = await _unitOfWork.AccountRepository.Get(accountId);

        if (account is null)
            return NotFound("Account not found");

        return Ok(new { account.Balance });
    }

    [Authorize]
    [HttpPost("transfer")]
    public async Task<ActionResult> Transfer([FromBody] TransferRequest transfer)
    {
        var origin = await _unitOfWork.AccountRepository.Get(transfer.Origin);
        var destiny = await _unitOfWork.AccountRepository.Get(transfer.Target);

        if (origin is null || destiny is null)
            return BadRequest();

        origin.Balance -= transfer.Amount;
        destiny.Balance += transfer.Amount;

        _unitOfWork.AccountRepository.Update(origin);
        _unitOfWork.AccountRepository.Update(destiny);

        var history = new HistoryDetail(transfer.Amount, DateTime.Now, TransactionType.Transfer, origin.Id, destiny.Id);
        _unitOfWork.AccountRepository.SaveHistory(history);

        await _unitOfWork.CommitAsync();

        return Ok(new
        {
            history.Date,
            Type = Enum.GetName<TransactionType>(history.Type),
            history.Amount,
            history.OriginId,
            history.DestinyId
        });
    }
}
