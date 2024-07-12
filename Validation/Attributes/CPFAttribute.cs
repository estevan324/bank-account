using System.ComponentModel.DataAnnotations;

namespace BankAccount.Validation.Attributes;

public class CPFAttribute : ValidationAttribute
{

    public override bool IsValid(object? value)
    {
        if (value is null) return false;
        string cpf = value.ToString() ?? string.Empty;
        return cpf.Length == 11 && cpf.All(char.IsDigit);
    }
}
