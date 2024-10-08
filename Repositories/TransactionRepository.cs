using System;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTrackerAPI.Data;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Repositories;

public class TransactionRepository : ITransactionRepository
{
  private readonly AppDbContext _context;
  public TransactionRepository(AppDbContext appContext)
  {
    _context = appContext;
  }
  public async Task<Transactions> AddAsync(Transactions transaction)
  {
    await _context.Transactions.AddAsync(transaction);
    await _context.SaveChangesAsync();
    return transaction;
  }

  public async Task<PaginatedList<Transactions>> GetAllTransactionsAsync(string userId, int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? transactionType)
  {
    var query = _context.Transactions.AsQueryable().Where(t => t.UserId == userId);

    // Convertir startDate a UTC si es necesario
    if (startDate.HasValue)
    {
      startDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
      query = query.Where(t => t.Date >= startDate);
    }

    // Convertir endDate a UTC si es necesario
    if (endDate.HasValue)
    {
      endDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
      query = query.Where(t => t.Date <= endDate);
    }

    // Filtrar por categoría si es necesario
    if (!string.IsNullOrEmpty(categoryName))
    {
      query = query.Where(t => t.Category.Name == categoryName);
    }

    // Filtrar por tipo de transacción si es necesario
    if (!string.IsNullOrEmpty(transactionType))
    {
      query = query.Where(t => t.TransactionType == transactionType);
    }

    // Ordenar por fecha
    query = query.OrderBy(t => t.Date);

    // Conteo total de elementos sin paginar
    var totalCount = await query.CountAsync();

    // Aplicar la paginación
    var transactions = await query
      .AsNoTracking()
      .Skip((page - 1) * results)
      .Take(results)
      .ToListAsync();

    return new PaginatedList<Transactions>(transactions, totalCount, page, results);
  }

  public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsByUserAsync(string userId)
  {
    return await _context.Transactions
          .Where(t => t.UserId == userId)
          .Include(t => t.Category)
          .Select(t => new TransactionDTO(
              t.Amount,
              t.TransactionType,
              t.CategoryId,
              t.Date,
              t.Description
          ))
          .ToListAsync();
  }

  public async Task<Transactions> GetByIdAsync(string id, string userId)
  {
    return await _context.Transactions
       .Where(t => t.TransactionId == id && t.UserId == userId)
       .FirstOrDefaultAsync();
  }

  public async Task<Transactions> RemoveAync(string id, string userId)
  {
    var transactionById = await GetByIdAsync(id, userId);
    if (transactionById is null)
    {
      return null;
    }
    _context.Transactions.Remove(transactionById);
    await _context.SaveChangesAsync();
    return transactionById;
  }

  public async Task<Transactions> UpdateAync(string id, TransactionDTO transactionDTO, string userId)
  {
    var transactionById = await GetByIdAsync(id, userId);
    if (transactionById is null)
    {
      return null;
    };
    _context.Entry(transactionById).CurrentValues.SetValues(transactionDTO);
    await _context.SaveChangesAsync();

    return transactionById;
  }
}
