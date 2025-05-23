using System.ComponentModel;
using System.Reflection;
using FluentValidation;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Validations;

public static class EnumExtensions
{
    public static string GetDescriptionsAsString(Type enumType)
    {
        var descriptions = new List<string>();
        
        foreach (var value in Enum.GetValues(enumType))
        {
            var field = enumType.GetField(value.ToString());
            var description = field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
            descriptions.Add(description ?? value.ToString());
        }
        
        return string.Join(", ", descriptions);
    }
}
public class AccountDTOValidator:AbstractValidator<AccountDTO>
{
    public AccountDTOValidator()
    {
        RuleFor(x => x.AccountName)
            .NotEmpty().WithMessage("El nombre cuenta es requerido.")
            .NotNull().WithMessage("El nombre no debe ser null.")
            .Length(3, 100).WithMessage("El nombre de la cuenta debe tener entre 3 y 100 caracteres.");
        
        RuleFor(x => x.AccountType)
            .NotEmpty().WithMessage("El tipo de cuenta es requerido.")
            .NotNull().WithMessage("El tipo de cuenta no debe ser null.")
            .IsInEnum().WithMessage($"El tipo de cuenta no es válido. Debe ser uno de: {EnumExtensions.GetDescriptionsAsString(typeof(AccountType))}");
        
        RuleFor(x => x.CurrentBalance)
            .NotEmpty().WithMessage("El saldo actual es requerido.")
            .NotNull().WithMessage("El saldo actual no debe ser null.")
            .InclusiveBetween(1, 100_000_000).WithMessage("El saldo actual debe estar entre $1 y $100000000.");
        
        RuleFor(x => x.InitialBalance)
            .NotEmpty().WithMessage("El saldo actual es requerido.")
            .NotNull().WithMessage("El saldo inicial no debe ser null.")
            .InclusiveBetween(1, 100_000_000).WithMessage("El saldo inicial debe estar entre $1 y $100000000.");
    }
}