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
    try
    {
      await _context.Transactions.AddAsync(transaction);
      await _context.SaveChangesAsync();
      return transaction;
    }
    catch (DbUpdateException ex)
    {
      throw new InvalidOperationException("Ocurrió un error al guardar la transacción.", ex);
    }
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error inesperado al agregar la transacción.", ex);
    }
  }

  public async Task<PaginatedList<Transactions>> GetAllTransactionsAsync(string userId, int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? transactionType)
  {
    try
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
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error al obtener las transacciones.", ex);
    }
  }

  public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsByUserAsync(string userId)
  {
    try
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
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error al obtener las transacciones del usuario.", ex);

    }
  }

  public async Task<Transactions> GetByIdAsync(string id, string userId)
  {
    try
    {
      var transaction = await _context.Transactions
         .Where(t => t.TransactionId == id && t.UserId == userId)
         .FirstOrDefaultAsync();
      if (transaction is null)
      {
        throw new KeyNotFoundException("Transacción no encontrada para el usuario especificado.");
      }
      return transaction;

    }
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error al obtener la transacción.", ex);
    }
  }

  public async Task<Transactions> RemoveAync(string id, string userId)
  {
    var transactionById = await GetByIdAsync(id, userId);
    if (transactionById is null)
    {
      throw new KeyNotFoundException("La transaccion especifica no se encontro");
    }
    _context.Transactions.Remove(transactionById);
    await _context.SaveChangesAsync();
    return transactionById;
  }

  public async Task<Transactions> UpdateAync(string id, TransactionDTO transactionDTO, string userId)
  {
    try
    {
      var transactionById = await GetByIdAsync(id, userId);
      if (transactionById is null)
      {
        throw new KeyNotFoundException("La transaccion especifica no se encontro");
      }
      _context.Entry(transactionById).CurrentValues.SetValues(transactionDTO);
      await _context.SaveChangesAsync();

      return transactionById;
    }
    catch (Exception ex)
    {
      throw new Exception("Ocurrió un error al actualizar la transaccion.", ex);
    }
  }
}
