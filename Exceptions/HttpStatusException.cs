using System.Net;

namespace BankAccount.Exceptions;

public class HttpStatusException : Exception
{
    public HttpStatusCode Status { get; set; }
    public HttpStatusException(HttpStatusCode status, string message) : base(message)
    {
        Status = status;
    }
}
