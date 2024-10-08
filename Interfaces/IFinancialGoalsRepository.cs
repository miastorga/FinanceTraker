using System;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface IFinancialGoalsRepository
{
  Task<FinancialGoal> AddAsync(FinancialGoal financialGoal);
  Task<FinancialGoal> GetByIdAsync(string id, string userId);
  Task<FinancialGoal> RemoveAync(string id, string userId);
  Task<FinancialGoal> UpdateAync(string id, FinancialGoalDTO financialGoalDTO, string userId);
  Task<PaginatedList<FinancialGoal>> GetAllFinancialGoalsAsync(int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? period, int goalAmount, string userId);
}
