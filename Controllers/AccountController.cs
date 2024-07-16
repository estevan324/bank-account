using BankAccount.DTOs.Account;
using BankAccount.DTOs.Clients;
using BankAccount.Entities;
using BankAccount.Entities.Enums;
using BankAccount.ExtensionMethods;
using BankAccount.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BankAccount.Exceptions;

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
            throw new NotFoundException("Account not found");

        return Ok(new { account.Balance });
    }


    [Authorize]
    [HttpPost("transfer")]
    public async Task<ActionResult<TransferResponse>> Transfer([FromBody] TransferRequest transfer)
    {
        var origin = await _unitOfWork.AccountRepository.GetAsync(a => a.Id == transfer.Origin);
        var destiny = await _unitOfWork.AccountRepository.GetAsync(a => a.Id == transfer.Target);

        if (origin is null || destiny is null)
            throw new BadRequestException("You must inform the origin and target accounts correctly");

        if (origin.Balance < transfer.Amount)
            InsufficientBalance();

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
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.Id == deposit.Target) ?? throw new NotFoundException("Account not found, you must inform the id correctly");

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
        var id = GetUserId();
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.UserId == id) ?? throw new NotFoundException("Account not found, you must inform the id correctly");

        if (account.Balance < withdraw.Amount)
            InsufficientBalance();

        account.Balance -= withdraw.Amount;

        var history = new HistoryDetail(withdraw.Amount, DateTime.Now, TransactionType.Withdraw, account.Id, account.Id);

        _unitOfWork.AccountRepository.Update(account);
        _unitOfWork.AccountRepository.SaveHistory(history);
        await _unitOfWork.CommitAsync();

        return Ok(history.ConvertToTransferResponse());
    }

    [Authorize]
    [HttpGet("historic")]
    public async Task<ActionResult> GetHistories([FromQuery] HistoricRequest request)
    {
        var userId = GetUserId();

        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.UserId == userId) ?? throw new NotFoundException("Account not found, you must inform the id correctly");

        var histories = await _unitOfWork.AccountRepository.GetHistoriesAsync(request.Page, request.PageLimit, account.Id);

        return Ok(histories.ConvertToTransferResponse());
    }

    private string GetUserId()
    {
        var userClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!;

        return userClaim.Value;
    }

    private static void InsufficientBalance()
    {
        throw new BadRequestException("Insufficient balance to complete the transaction.");
    }
}
