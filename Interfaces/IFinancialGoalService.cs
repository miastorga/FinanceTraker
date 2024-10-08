using System;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface IFinancialGoalService
{
  Task<FinancialGoalDTO> CreateFinancialGoalAsync(FinancialGoalDTO financialGoalDTO, string userId);
  Task<FinancialGoalDTO> GetFinancialGoalByIdAsync(string id, string userId);
  Task<FinancialGoalDTO> RemoveFinancialGoalAsync(string id, string userId);
  Task<FinancialGoalDTO> UpdateFinancialGoalAsync(string id, FinancialGoalDTO financialGoalDTO, string userId);
  Task<PaginatedList<FinancialGoal>> GetAllFinancialGoals(int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? period, int goalAmount, string userId);

}
