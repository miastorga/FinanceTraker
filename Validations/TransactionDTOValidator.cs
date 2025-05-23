using FluentValidation;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Validations;

public class TransactionDTOValidator:AbstractValidator<TransactionDTO>
{
    public TransactionDTOValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("La cantidad es requerida.")
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("La cantidad no puede exceder $1000000000.");

        RuleFor(x => x.TransactionType)
            .NotEmpty().WithMessage("El tipo de transaccion es requerido.")
            .Must(BeValidTransactionType).WithMessage("El tipo de transaccion debe ser 'ingreso' o 'gasto'.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID es requerido.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La fecha es requerida.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description no puede exceder los 500 caracteres."); 

           RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("El ID de categoría es requerido.");
         
    }
    
    private bool BeValidTransactionType(string transactionType)
    {
        return transactionType.Equals("ingreso", StringComparison.OrdinalIgnoreCase) ||
               transactionType.Equals("gasto", StringComparison.OrdinalIgnoreCase);
    }
}
