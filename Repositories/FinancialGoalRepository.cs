using System;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTrackerAPI.Data;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Repositories;

public class FinancialGoalRepository : IFinancialGoalsRepository
{
  private readonly AppDbContext _context;
  public FinancialGoalRepository(AppDbContext appContext)
  {
    _context = appContext;
  }
  public async Task<FinancialGoal> AddAsync(FinancialGoal financialGoal)
  {
    await _context.FinancialGoals.AddAsync(financialGoal);
    await _context.SaveChangesAsync();
    return financialGoal;
  }

  public async Task<PaginatedList<FinancialGoal>> GetAllFinancialGoalsAsync(int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? period, int goalAmount, string userId)
  {
    // Iniciar la consulta base
    var query = _context.FinancialGoals.AsQueryable().Where(t => t.UserId == userId);

    // Filtro por fechas
    if (startDate.HasValue)
    {
      startDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
      query = query.Where(g => g.StartDate >= startDate);
    }

    if (endDate.HasValue)
    {
      endDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
      query = query.Where(g => g.EndDate <= endDate);
    }

    // Filtro por categoría
    if (!string.IsNullOrEmpty(categoryName))
    {
      query = query.Where(g => g.Category.Name == categoryName);
    }

    // Filtro por periodo
    if (!string.IsNullOrEmpty(period))
    {
      query = query.Where(g => g.Period == period);
    }

    // Filtro por GoalAmount
    if (goalAmount > 0)
    {
      query = query.Where(g => g.GoalAmount == goalAmount);
    }

    // Ordenar por fecha de inicio (opcional)
    query = query.OrderBy(g => g.StartDate);

    // Obtener el conteo total sin paginación
    var totalCount = await query.CountAsync();

    // Aplicar la paginación
    var goals = await query
        .AsNoTracking()
        .Skip((page - 1) * results)
        .Take(results)
        .ToListAsync();

    // Crear y devolver el objeto paginado
    return new PaginatedList<FinancialGoal>(goals, totalCount, page, results);
  }

  public async Task<FinancialGoal> GetByIdAsync(string id, string userId)
  {
    return await _context.FinancialGoals
        .FirstOrDefaultAsync(g => g.FinancialGoalId == id && g.UserId == userId);
  }

  public async Task<FinancialGoal> RemoveAync(string id, string userId)
  {
    var financialGoalById = await GetByIdAsync(id, userId);
    if (financialGoalById is null)
    {
      return null;
    }
    _context.FinancialGoals.Remove(financialGoalById);
    await _context.SaveChangesAsync();
    return financialGoalById;
  }

  public async Task<FinancialGoal> UpdateAync(string id, FinancialGoalDTO financialGoalDTO, string userId)
  {
    var financialGoalById = await GetByIdAsync(id, userId);
    if (financialGoalById is null)
    {
      return null;
    };
    _context.Entry(financialGoalById).CurrentValues.SetValues(financialGoalDTO);
    await _context.SaveChangesAsync();

    return financialGoalById;
  }
}
