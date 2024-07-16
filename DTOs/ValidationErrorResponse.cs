namespace BankAccount.DTOs;

public record ValidationErrorResponse(string Type, string Title, int Status, Dictionary<string, string[]> Errors, string TraceId);
