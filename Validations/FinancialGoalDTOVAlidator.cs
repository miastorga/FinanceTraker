using FluentValidation;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Validations;

public class FinancialGoalDTOVAlidator:AbstractValidator<FinancialGoalDTO>
{
    public FinancialGoalDTOVAlidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("El ID de categoría es requerido.");
         
        RuleFor(x => x.GoalAmount)
            .NotNull()
            .WithMessage("El monto de la meta es requerido.")
            .GreaterThan(0)
            .WithMessage("El monto de la meta debe ser mayor a cero.")
            .LessThanOrEqualTo(1_000_000_000)
            .WithMessage("El monto de la meta no puede exceder $1000000000.");
        
        RuleFor(x => x.Period)
            .NotEmpty()
            .WithMessage("El período es requerido.")
            .Must(period => new[] { "diario", "semanal", "mensual", "anual" }.Contains(period?.ToLower()))
            .WithMessage("El período debe ser: diario, semanal, mensual o anual.");

        
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("La fecha de inicio es requerida.")
            .GreaterThanOrEqualTo(DateTime.Today.AddDays(-30))
            .WithMessage("La fecha de inicio no puede ser más de 30 días en el pasado.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("La fecha de fin es requerida.")
            .GreaterThan(x => x.StartDate)
            .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");
        
        When(x => x.Period?.ToLower() == "semanal", () => {
            RuleFor(x => x)
                .Must(goal => (goal.EndDate - goal.StartDate).TotalDays >= 7)
                .WithMessage("Una meta semanal debe durar al menos 7 días.")
                .WithName("WeeklyGoalMinDuration");
        });
        
        When(x => x.Period?.ToLower() == "mensual", () => {
            RuleFor(x => x)
                .Must(goal => (goal.EndDate - goal.StartDate).TotalDays >= 28)
                .WithMessage("Una meta mensual debe durar al menos 28 días.")
                .WithName("MonthlyGoalMinDuration");
        });
        
        When(x => x.Period?.ToLower() == "anual", () => {
            RuleFor(x => x)
                .Must(goal => (goal.EndDate - goal.StartDate).TotalDays <= 1825) // 5 años
                .WithMessage("Una meta anual no debería durar más de 5 años.")
                .WithName("YearlyGoalMaxDuration");
        });
        
        RuleFor(x => x)
            .Must(goal => (goal.EndDate - goal.StartDate).TotalDays >= 1)
            .WithMessage("La meta debe durar al menos 1 día.")
            .WithName("MinimumDuration");

        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.Today)
            .WithMessage("Una meta financiera debe tener fecha de finalización futura.");

    }
}