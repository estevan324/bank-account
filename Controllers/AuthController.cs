using BankAccount.DTOs.Auth;
using BankAccount.Services.Interfaces;
using BankAccount.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankAccount.Controllers;

[Route("api/[controller]")]
[ApiController]
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

    [HttpPost("login")]
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
