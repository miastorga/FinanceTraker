using System;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface ITransactionRepository
{
  Task<Transactions> AddAsync(Transactions transaction);
  Task<Transactions> GetByIdAsync(string id, string userId);
  Task<Transactions> RemoveAync(string id, string userId);
  Task<Transactions> UpdateAync(string id, TransactionDTO transactionDTO, string userId);
  Task<PaginatedList<Transactions>> GetAllTransactionsAsync(string userId, int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? transactionType);
  Task<IEnumerable<TransactionDTO>> GetAllTransactionsByUserAsync(string userId);
}
