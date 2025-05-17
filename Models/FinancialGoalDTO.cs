using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTrackerAPI.Models;

public record FinancialGoalDTO
(
  [Required(ErrorMessage = "Category ID is required.")]
 string CategoryId,

  [Required(ErrorMessage = "Goal amount is required.")]
  [Range(1, int.MaxValue, ErrorMessage = "Goal amount must be greater than zero.")]
 int GoalAmount,
  [Required(ErrorMessage = "Period is required")]
  [AllowedValues("diario", "semanal", "mensual", "anual", ErrorMessage = "Period must be diario, semanal, mensual or anual")]
 string Period,
  [Required(ErrorMessage = "Start date is required.")]
 DateTime StartDate,
  [Required(ErrorMessage = "End date is required.")]
 DateTime EndDate
);
