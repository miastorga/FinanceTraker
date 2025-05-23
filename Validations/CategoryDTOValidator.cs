using FluentValidation;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Validations;

internal sealed class CategoryDTOValidator:AbstractValidator<CategoryDTO>
{
    public CategoryDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre categoria es requerido.")
            .NotNull().WithMessage("El nombre no debe ser null.")
            .Length(3, 40).WithMessage("El nombre de la categoria debe tener entre 3 y 40 caracteres.");
    }
}