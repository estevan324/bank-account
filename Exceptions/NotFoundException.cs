using System.Net;

namespace BankAccount.Exceptions;

public class NotFoundException : HttpStatusException
{
    public NotFoundException(string message) : base(HttpStatusCode.NotFound, message)
    {   
    }
}
