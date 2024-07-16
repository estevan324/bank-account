using System.Net;

namespace BankAccount.Exceptions;

public class UnauthorizedException : HttpStatusException
{
    public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message)
    {
    }
}
