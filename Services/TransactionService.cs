using System;
using System.Transactions;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Services;

public class TransactionService : ITransactionService
{
  private readonly ITransactionRepository _transactionRepository;
  private readonly ICategoryService _categoryService;
  public TransactionService(ITransactionRepository transaction, ICategoryService categoryService)
  {
    _transactionRepository = transaction;
    _categoryService = categoryService;
  }
  public async Task<TransactionResponseDTO> CreateTransactionAsync(TransactionDTO transactionDto, string userId)
  {
    var transaction = new Transactions
    {
      TransactionId = Guid.NewGuid().ToString(),
      UserId = userId,
      Amount = transactionDto.Amount,
      TransactionType = transactionDto.TransactionType,
      CategoryId = transactionDto.CategoryId,
      Date = transactionDto.Date,
      Description = transactionDto.Description,
    };

    await _transactionRepository.AddAsync(transaction);
    var category = await _categoryService.GetCategoryByIdAsync(transaction.CategoryId, userId);
    if (category is null)
    {
      throw new KeyNotFoundException("La categoria especifica no fue encontrada");
    }

    var response = new TransactionResponseDTO(
          transaction.TransactionId,
          transaction.UserId,
          transaction.Amount,
          transaction.TransactionType,
          transaction.CategoryId,
          category.Name,
          transaction.Date,
          transaction.Description
      );

    return response;
  }

  public async Task<PaginatedList<Transactions>> GetAllTransactions(string userId, int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? transactionType)
  {
    return await _transactionRepository.GetAllTransactionsAsync(userId, page, results, startDate, endDate, categoryName, transactionType);
  }

  public async Task<TransactionResponseDTO> GetTransactionByIdAsync(string id, string userId)
  {
    var transaction = await _transactionRepository.GetByIdAsync(id, userId);
    if (transaction is null)
    {
      throw new KeyNotFoundException("La transaccion especifica no fue encontrada");
    }

    var category = await _categoryService.GetCategoryByIdAsync(transaction.CategoryId, userId);
    if (category is null)
    {
      throw new KeyNotFoundException("La categoria especifica no fue encontrada");
    }

    var response = new TransactionResponseDTO(
           transaction.TransactionId,
           transaction.UserId,
           transaction.Amount,
           transaction.TransactionType,
           transaction.CategoryId,
           category.Name,
           transaction.Date,
           transaction.Description
       );

    return response;
  }

  public async Task<Summary> GetTransactionSummaryAsync(string userId)
  {
    var transactions = await _transactionRepository.GetAllTransactionsByUserAsync(userId);

    // Obtener los nombres de categorías desde la base de datos, si no se tiene en los DTOs
    var categories = await _categoryService.GetAllCategoriesAsync(userId);
    var categoryNames = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);

    // Calcular ingresos, gastos y balances
    var totalIncome = transactions
        .Where(t => t.TransactionType == "ingreso")
        .Sum(t => t.Amount);

    var totalExpenses = transactions
        .Where(t => t.TransactionType == "gasto")
        .Sum(t => t.Amount);

    var balance = totalIncome - totalExpenses;

    // Agrupar por categorías para los gastos
    var expensesByCategory = transactions
        .Where(t => t.TransactionType == "gasto")
        .GroupBy(t => t.CategoryId)
        .ToDictionary(
            g => categoryNames[g.Key], // Acceder al nombre de la categoría usando el CategoryId
            g => g.Sum(t => t.Amount)
        );

    // Agrupar por categorías para los ingresos
    var incomeByCategory = transactions
        .Where(t => t.TransactionType == "ingreso")
        .GroupBy(t => t.CategoryId)
        .ToDictionary(
            g => categoryNames[g.Key], // Acceder al nombre de la categoría usando el CategoryId
            g => g.Sum(t => t.Amount)
        );

    return new Summary
    {
      TotalIncome = totalIncome,
      TotalExpenses = totalExpenses,
      Balance = balance,
      ExpensesByCategory = expensesByCategory,
      IncomeByCategory = incomeByCategory
    };
  }

  public async Task<TransactionDTO> RemoveTransactionAsync(string id, string userId)
  {
    var transaction = await _transactionRepository.RemoveAync(id, userId);
    if (transaction is null)
    {
      return null;
    }
    var transanctionResponse = new TransactionDTO
    (
      transaction.Amount,
      transaction.CategoryId,
      transaction.TransactionType,
      transaction.Date,
      transaction.Description
    );
    return transanctionResponse;
  }

  public async Task<TransactionDTO> UpdateTransactionAsync(string id, TransactionDTO transactionDto, string userId)
  {
    var updatedTransaction = await _transactionRepository.UpdateAync(id, transactionDto, userId);
    if (updatedTransaction is null)
    {
      return null;
    }
    var updatedTransactionDTO = new TransactionDTO
       (
         updatedTransaction.Amount,
         updatedTransaction.TransactionType,
         updatedTransaction.CategoryId,
         updatedTransaction.Date,
         updatedTransaction.Description
       );

    return updatedTransactionDTO;
  }
}
