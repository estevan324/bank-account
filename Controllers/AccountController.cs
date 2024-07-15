using BankAccount.DTOs.Account;
using BankAccount.DTOs.Clients;
using BankAccount.Entities;
using BankAccount.Entities.Enums;
using BankAccount.ExtensionMethods;
using BankAccount.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.Id == accountId);

        if (account is null)
            return NotFound("Account not found");

        return Ok(new { account.Balance });
    }

    [Authorize]
    [HttpPost("transfer")]
    public async Task<ActionResult<TransferResponse>> Transfer([FromBody] TransferRequest transfer)
    {
        var origin = await _unitOfWork.AccountRepository.GetAsync(a => a.Id == transfer.Origin);
        var destiny = await _unitOfWork.AccountRepository.GetAsync(a => a.Id == transfer.Target);

        if (origin is null || destiny is null)
            return BadRequest();

        if (origin.Balance < transfer.Amount) 
            throw new InvalidOperationException("Insufficient balance");
        origin.Balance -= transfer.Amount;
        destiny.Balance += transfer.Amount;

        var history = new HistoryDetail(transfer.Amount, DateTime.Now, TransactionType.Transfer, origin.Id, destiny.Id);

        _unitOfWork.AccountRepository.Update(origin);
        _unitOfWork.AccountRepository.Update(destiny);
        _unitOfWork.AccountRepository.SaveHistory(history);
        await _unitOfWork.CommitAsync();

        return Ok(history.ConvertToTransferResponse());
    }

    [Authorize]
    [HttpPost("deposit")]
    public async Task<ActionResult<TransferResponse>> Deposit([FromBody] DepositRequest deposit)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.Id == deposit.Target);
        if (account is null)
            return BadRequest();

        account.Balance += deposit.Amount;

        var history = new HistoryDetail(deposit.Amount, DateTime.Now, TransactionType.Deposit, account.Id, account.Id);

        _unitOfWork.AccountRepository.Update(account);
        _unitOfWork.AccountRepository.SaveHistory(history);
        await _unitOfWork.CommitAsync();

        return Ok(history.ConvertToTransferResponse());
    }

    [Authorize]
    [HttpPost("withdraw")]
    public async Task<ActionResult<TransferResponse>> Withdraw(WithdrawRequest withdraw)
    {
        var userClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userClaim is null)
            return BadRequest();

        var id = userClaim.Value;
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.UserId == id);
        if (account is null) return BadRequest();

        if (account.Balance < withdraw.Amount)
            return BadRequest("Insufficient balance");

        account.Balance -= withdraw.Amount;

        var history = new HistoryDetail(withdraw.Amount, DateTime.Now, TransactionType.Withdraw, account.Id, account.Id);

        _unitOfWork.AccountRepository.Update(account);
        _unitOfWork.AccountRepository.SaveHistory(history);
        await _unitOfWork.CommitAsync();

        return Ok(history.ConvertToTransferResponse());
    }
}
