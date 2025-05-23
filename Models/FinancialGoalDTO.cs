using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTrackerAPI.Models;

public record FinancialGoalDTO
(
 string CategoryId,

 int GoalAmount,
 string Period,
 DateTime StartDate,
 DateTime EndDate
);
