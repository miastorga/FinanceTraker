using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTrackerAPI.Models;

public class FinancialGoal
{
  [Key]
  public string FinancialGoalId { get; set; }

  [Required(ErrorMessage = "User ID is required.")]
  public string UserId { get; set; }
  public AppUser User { get; set; }

  [Required(ErrorMessage = "Category ID is required.")]
  public string CategoryId { get; set; }
  public Category Category { get; set; }

  [Required(ErrorMessage = "Goal amount is required.")]
  [Range(1, int.MaxValue, ErrorMessage = "Goal amount must be greater than zero.")]
  public int GoalAmount { get; set; }

  [Required(ErrorMessage = "Period is required")]
  [AllowedValues("diario", "semanal", "mensual", "anual")]
  public string Period { get; set; }

  [Required(ErrorMessage = "Start date is required.")]
  public DateTime StartDate { get; set; }

  [Required(ErrorMessage = "End date is required.")]
  public DateTime EndDate { get; set; }
}
