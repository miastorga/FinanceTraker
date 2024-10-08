using System;

namespace PersonalFinanceTrackerAPI.Models;

public class Summary
{
  public decimal TotalIncome { get; set; }
  public decimal TotalExpenses { get; set; }
  public decimal Balance { get; set; }
  public Dictionary<string, int> ExpensesByCategory { get; set; }
  public Dictionary<string, int> IncomeByCategory { get; set; }
}
