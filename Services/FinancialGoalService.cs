using System;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Services;

public class FinancialGoalService : IFinancialGoalService
{
  private readonly IFinancialGoalsRepository _financialGoalRepository;
  private readonly ICategoryRepository _categoryRepository;

  public FinancialGoalService(IFinancialGoalsRepository financialGoalsRepository, ICategoryRepository categoryRepository)
  {
    _financialGoalRepository = financialGoalsRepository;
    _categoryRepository = categoryRepository;
  }
  public async Task<FinancialGoalDTO> CreateFinancialGoalAsync(FinancialGoalDTO financialGoalDTO, string userId)
  {
    var category = await _categoryRepository.GetByIdAsync(financialGoalDTO.CategoryId, userId);
    if (category is null)
    {
      throw new Exception("La categoria especificada no existe");
    }

    var financialGoal = new FinancialGoal
    {
      FinancialGoalId = Guid.NewGuid().ToString(),
      UserId = userId,
      CategoryId = financialGoalDTO.CategoryId,
      GoalAmount = financialGoalDTO.GoalAmount,
      Period = financialGoalDTO.Period,
      StartDate = financialGoalDTO.StartDate,
      EndDate = financialGoalDTO.EndDate,
    };

    await _financialGoalRepository.AddAsync(financialGoal);

    var response = new FinancialGoalDTO(
          financialGoal.CategoryId,
          financialGoal.GoalAmount,
          financialGoal.Period,
          financialGoal.StartDate,
          financialGoal.EndDate
      );

    return response;
  }

  public async Task<PaginatedList<FinancialGoal>> GetAllFinancialGoals(int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? period, int goalAmount, string userId)
  {
    return await _financialGoalRepository.GetAllFinancialGoalsAsync(page, results, startDate, endDate, categoryName, period, goalAmount, userId);
  }

  public async Task<FinancialGoalDTO> GetFinancialGoalByIdAsync(string id, string userId)
  {
    var financialGoal = await _financialGoalRepository.GetByIdAsync(id, userId);

    if (financialGoal is null)
    {
      return null;
    }

    var response = new FinancialGoalDTO(
           financialGoal.CategoryId,
           financialGoal.GoalAmount,
           financialGoal.Period,
           financialGoal.StartDate,
           financialGoal.EndDate
       );

    return response;
  }

  public async Task<FinancialGoalDTO> RemoveFinancialGoalAsync(string id, string userId)
  {
    var financialGoal = await _financialGoalRepository.RemoveAync(id, userId);
    if (financialGoal is null)
    {
      return null;
    }
    var financialGoalResponse = new FinancialGoalDTO
    (
      financialGoal.CategoryId,
      financialGoal.GoalAmount,
      financialGoal.Period,
      financialGoal.StartDate,
      financialGoal.EndDate
    );
    return financialGoalResponse;
  }

  public async Task<FinancialGoalDTO> UpdateFinancialGoalAsync(string id, FinancialGoalDTO financialGoalDTO, string userId)
  {
    var updatedFinancialGoal = await _financialGoalRepository.UpdateAync(id, financialGoalDTO, userId);
    if (updatedFinancialGoal is null)
    {
      return null;
    }
    var updatedFinancialGoalDto = new FinancialGoalDTO
       (
         updatedFinancialGoal.CategoryId,
         updatedFinancialGoal.GoalAmount,
         updatedFinancialGoal.Period,
         updatedFinancialGoal.StartDate,
         updatedFinancialGoal.EndDate
       );

    return updatedFinancialGoalDto;
  }
}
