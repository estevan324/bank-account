using System.Net;

namespace BankAccount.Exceptions;

public class BadRequestException : HttpStatusException
{
	public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message)
	{
	}
}
