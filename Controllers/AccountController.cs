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
using BankAccount.DTOs;

namespace BankAccount.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    public AccountController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Registra uma nova conta bancária
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST api/account
    ///     
    ///     {
    ///         "name": "My name",
    ///         "cpf": "12345678901",
    ///         "password": "MyPassword",
    ///         "initialBalance": 1000
    ///     }
    /// </remarks>
    /// <param name="client">Objeto cliente</param>
    /// <returns>Um identificador único da conta bancária criada</returns>
    /// <response code="201">Retorna o ID da conta bancária criada</response>
    /// <response code="400">Retorna um erro se a requisição for inválida</response>
    /// <response code="500">Retorna um erro interno do servidor</response>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateAccount([FromBody] ClientRegisterRequest client)
    {
        var userDto = new UserDetail(client.Name!, client.Cpf!, client.Password!);
        var userCreated = await _unitOfWork.CustomerRepository.CreateAsync(userDto);

        var account = _unitOfWork.AccountRepository.Create(new Account(client.InitialBalance, userCreated.Id));
        await _unitOfWork.CommitAsync();

        return Created("/account", account.Id);
    }

    /// <summary>
    /// Retorna o saldo de uma conta bancária
    /// </summary>
    /// <param name="accountId">Identificador único da conta</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     GET api/account/balance?accountId=3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </remarks>
    /// <returns>O saldo atual da conta informada</returns>
    /// <exception cref="NotFoundException"></exception>
    /// <response code="200">Retorna o saldo atual da conta bancária informada</response>
    /// <response code="400">Retorna um erro se a requisição for inválida</response>
    /// <response code="404">Retorna um erro caso não encontre a conta informada</response>
    /// <response code="500">Retorna um erro interno do servidor</response>
    [Authorize]
    [HttpGet("balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BalanceResponse>> ShowBalance([FromQuery] Guid accountId)
    {
        var account = await _unitOfWork.AccountRepository.GetAsync(a => a.Id == accountId);

        if (account is null)
            throw new NotFoundException("Account not found");

        return Ok(new BalanceResponse(account.Balance));
    }

    /// <summary>
    /// Realiza a transferência de fundos entre duas contas
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST api/account/transfer
    ///     {
    ///         "origin": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "target": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "amount": 10
    ///     }
    /// </remarks>
    /// <param name="transfer">Dados para a transação</param>
    /// <returns>Um objeto informando a data, valor e as contas bancárias da operação</returns>
    /// <exception cref="BadRequestException"></exception>
    /// <response code="200">Retorna um objeto com as informações da transferência</response>
    /// <response code="400">Retorna um erro se a requisição for inválida</response>
    /// <response code="500">Retorna um erro interno do servidor</response>
    [Authorize]
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Realiza o depósito de fundos para uma determinada conta
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST api/account/deposit
    ///     {
    ///         "target": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "amount": 10
    ///     }
    /// </remarks>
    /// <param name="deposit">Dados para o depósito</param>
    /// <returns>Um objeto informando a data, valor e as contas bancárias da operação</returns>
    /// <exception cref="NotFoundException"></exception>
    /// <response code="200">Retorna um objeto com as informações da operação</response>
    /// <response code="404">Retorna um erro se a conta de destino não for encontrada</response>
    /// <response code="400">Retorna um erro se a requisição for inválida</response>
    /// <response code="500">Retorna um erro interno do servidor</response>
    [Authorize]
    [HttpPost("deposit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Realiza o saque na conta bancária do usuário atual autenticado
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST api/account/withdraw
    ///     {
    ///         "amount": 10
    ///     }
    /// </remarks>
    /// <param name="withdraw">Dados para o saque</param>
    /// <returns>Um objeto informando a data, valor e as contas bancárias da operação</returns>
    /// <exception cref="NotFoundException"></exception>
    /// <response code="200">Retorna um objeto com as informações da transferência</response>
    /// <response code="404">Retorna um erro se a conta de destino não for encontrada</response>
    /// <response code="400">Retorna um erro se a requisição for inválida</response>
    /// <response code="500">Retorna um erro interno do servidor</response>
    [Authorize]
    [HttpPost("withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Retorna o histórico da conta bancária do usuário atual autenticado
    /// </summary>
    /// <param name="request">Parâmetros para paginação do histórico</param>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     GET api/account/history?page=1&amp;pageLimit=10
    /// </remarks>
    /// <returns>Uma lista de transações realizadas ou recebidas pelo usuário</returns>
    /// <exception cref="NotFoundException"></exception>
    /// <response code="200">Retorna uma lista de transações realizadas ou recebidas do usuário</response>
    /// <response code="400">Retorna um erro se a requisição for inválida</response>
    /// <response code="404">Retorna um erro caso não encontre a conta informada</response>
    /// <response code="500">Retorna um erro interno do servidor</response>
    [Authorize]
    [HttpGet("historic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TransferResponse>>> GetHistories([FromQuery] HistoricRequest request)
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
