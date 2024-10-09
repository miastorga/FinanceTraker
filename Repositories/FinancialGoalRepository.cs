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
    try
    {
      await _context.FinancialGoals.AddAsync(financialGoal);
      await _context.SaveChangesAsync();
      return financialGoal;
    }
    catch (DbUpdateException ex)
    {
      throw new InvalidOperationException("Ocurrió un error al guardar la meta financiera.", ex);
    }
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error inesperado al agregar la meta financiera.", ex);
    }
  }

  public async Task<PaginatedList<FinancialGoal>> GetAllFinancialGoalsAsync(int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? period, int goalAmount, string userId)
  {
    try
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
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error al obtener las metas financieras.", ex);
    }
  }

  public async Task<FinancialGoal> GetByIdAsync(string id, string userId)
  {
    try
    {
      var financialGoal = await _context.FinancialGoals
     .FirstOrDefaultAsync(g => g.FinancialGoalId == id && g.UserId == userId);
      if (financialGoal is null)
      {
        throw new KeyNotFoundException("Meta financiera no encontrada para el usuario especificado.");
      }
      return financialGoal;
    }
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error al obtener la meta finanriera.", ex);
    }
  }

  public async Task<FinancialGoal> RemoveAync(string id, string userId)
  {
    try
    {

      var financialGoalById = await GetByIdAsync(id, userId);
      if (financialGoalById is null)
      {
        throw new KeyNotFoundException("La meta financiera especifica no se encontro");
      }
      _context.FinancialGoals.Remove(financialGoalById);
      await _context.SaveChangesAsync();
      return financialGoalById;
    }
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error al remover la meta financiera.", ex);
    }
  }

  public async Task<FinancialGoal> UpdateAync(string id, FinancialGoalDTO financialGoalDTO, string userId)
  {
    try
    {
      var financialGoalById = await GetByIdAsync(id, userId);
      if (financialGoalById is null)
      {
        throw new KeyNotFoundException("La meta financiera especifica no se encontro");
      };
      _context.Entry(financialGoalById).CurrentValues.SetValues(financialGoalDTO);
      await _context.SaveChangesAsync();

      return financialGoalById;
    }
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error al actualizar la meta financiera.", ex);
    }
  }
}
