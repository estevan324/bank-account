using BankAccount.DTOs.Auth;
using BankAccount.Services.Interfaces;
using BankAccount.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BankAccount.DTOs;
using System.ComponentModel.DataAnnotations;

namespace BankAccount.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public AuthController(ITokenService tokenService, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Obtém um token JWT de autenticação para uso nos endpoints
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição
    /// 
    ///     POST api/auth/login
    ///     {
    ///         "cpf": "12345678901",
    ///         "password": "MyPassword"
    ///     }
    /// </remarks>
    /// <param name="login">Objeto LoginRequest</param>
    /// <returns>Um token JWT e a data de expiração</returns>
    /// <exception cref="UnauthorizedException"></exception>
    /// <response code="200">Retorna um objeto de token</response>
    /// <response code="400">Retorna um erro de validação caso os campos não foram digitados corretamente</response>
    /// <response code="401">Retorna um erro se as credenciais forem inválidas</response>
    /// <response code="500">Retorna uma mensagem de erro interno do servidor</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest login)
    {
        var user = await _userManager.FindByNameAsync(login.Cpf!);
        if (user is null || !(await _userManager.CheckPasswordAsync(user, login.Password!)))
            throw new UnauthorizedException("Invalid CPF or Password");

        var claims = await _userManager.GetClaimsAsync(user);

        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

        var token = _tokenService.GenerateAccessToken(claims, _configuration);
        var writtenToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new TokenResponse(writtenToken, token.ValidTo.ToLocalTime()));
    }
}
