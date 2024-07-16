using BankAccount.Exceptions;
using System.Net;

namespace BankAccount.Middlewares;

public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public GlobalErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
       try
        {
            await _next(context);
        } catch (Exception exc)
        {
            await HandleExceptionAsync(context, exc);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exc)
    {
        HttpStatusCode status;
        string message;

        if (exc is HttpStatusException httpStatusException)
        {
            status = httpStatusException.Status;
            message = httpStatusException.Message;
        } 
        else
        {
            status = HttpStatusCode.InternalServerError;
            message = exc.Message ?? "An unexpected error occurred";
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var errorResponse = new
        {
            StatusCode = (int)status,
            Message = message
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}
