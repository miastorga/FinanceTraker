using System;
using Microsoft.AspNetCore.Identity;

namespace PersonalFinanceTrackerAPI.Models;

public class AppUser : IdentityUser
{
  public ICollection<Transactions> Transactions { get; set; }
  public ICollection<FinancialGoal> FinancialGoals { get; set; } // Para la relación con FinancialGoal
  public ICollection<Category> Category { get; set; } // Para la relación con Category

}
