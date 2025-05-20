using System;
using Microsoft.EntityFrameworkCore.Storage;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface ITransactionRepository
{
  Task<IDbContextTransaction> BeginTransactionAsync();
  Task CommitTransactionAsync(IDbContextTransaction transaction);
  Task RollbackTransactionAsync(IDbContextTransaction transaction);
  Task<Transactions> AddAsync(Transactions transaction);
  Task<Transactions> GetByIdAsync(string id, string userId);
  void RemoveAync(Transactions transaction);
  void UpdateAync(Transactions transactions, TransactionDTO transactionDTO);
  Task<PaginatedList<TransactionResponseDTO>> GetAllTransactionsAsync(string userId, int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? transactionType);
  Task<IEnumerable<TransactionDTO>> GetAllTransactionsByUserAsync(string userId);
  Task SaveChangesAsync();
}
