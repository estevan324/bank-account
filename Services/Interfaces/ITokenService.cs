using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankAccount.Services.Interfaces;

public interface ITokenService
{
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config);
}
