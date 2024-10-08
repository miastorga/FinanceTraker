using System;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface ITransactionService
{
  Task<TransactionResponseDTO> CreateTransactionAsync(TransactionDTO transactionDto, string userId);
  Task<TransactionResponseDTO> GetTransactionByIdAsync(string id, string userId);
  Task<TransactionDTO> RemoveTransactionAsync(string id, string userId);
  Task<TransactionDTO> UpdateTransactionAsync(string id, TransactionDTO transactionDto, string userId);
  Task<PaginatedList<Transactions>> GetAllTransactions(string userId, int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? transactionType);
  Task<Summary> GetTransactionSummaryAsync(string userId);
}
