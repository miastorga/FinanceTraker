using System;
using System.Transactions;
using PersonalFinanceTrackerAPI.Data;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Services;

public class TransactionService : ITransactionService
{
  private readonly ITransactionRepository _transactionRepository;
  private readonly ICategoryService _categoryService;
  private readonly IAccountService _accountService;
  private readonly AppDbContext _context;
  public TransactionService(ITransactionRepository transaction, ICategoryService categoryService, IAccountService accountService, AppDbContext context)
  {
    _transactionRepository = transaction;
    _categoryService = categoryService;
    _accountService = accountService;
    _context = context;
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
      AccountId = string.IsNullOrEmpty(transactionDto.AccountId) ? null : transactionDto.AccountId
    };

    if (transactionDto.TransactionType == "ingreso" && !string.IsNullOrEmpty(transactionDto.AccountId))
    {
      var account = await _accountService.GetAccountByIdAsync(transactionDto.AccountId, userId);
      account.CurrentBalance += transactionDto.Amount;
      var updateAccount = await _accountService.UpdateAccountAsync(transactionDto.AccountId, account,userId);
      
      await _transactionRepository.AddAsync(transaction);
      var categoryById = await _categoryService.GetCategoryByIdAsync(transaction.CategoryId, userId);
      var responseDTO = new TransactionResponseDTO(
        transaction.TransactionId,
        transaction.Amount,
        transaction.TransactionType,
        transaction.CategoryId,
        categoryById.Name,
        transaction.Date,
        transaction.Description,
        transaction.AccountId
      );

      return responseDTO;
    }else if (transactionDto.TransactionType == "gasto" && !string.IsNullOrEmpty(transactionDto.AccountId))
    {
      var account = await _accountService.GetAccountByIdAsync(transactionDto.AccountId, userId);
      account.CurrentBalance -= transactionDto.Amount;
      var updateAccount = await _accountService.UpdateAccountAsync(transactionDto.AccountId, account,userId);
      
      await _transactionRepository.AddAsync(transaction);
      var categoryById = await _categoryService.GetCategoryByIdAsync(transaction.CategoryId, userId);
      var responseDTO = new TransactionResponseDTO(
        transaction.TransactionId,
        transaction.Amount,
        transaction.TransactionType,
        transaction.CategoryId,
        categoryById.Name,
        transaction.Date,
        transaction.Description,
        transaction.AccountId
      );

      return responseDTO;
    }
    
    await _transactionRepository.AddAsync(transaction);
    var category = await _categoryService.GetCategoryByIdAsync(transaction.CategoryId, userId);
    var response = new TransactionResponseDTO(
      transaction.TransactionId,
      transaction.Amount,
      transaction.TransactionType,
      transaction.CategoryId,
      category.Name,
      transaction.Date,
      transaction.Description,
      transaction.AccountId
    );

    return response;
  }

  public async Task<PaginatedList<TransactionResponseDTO>> GetAllTransactions(string userId, int page, int results, DateTime? startDate, DateTime? endDate, string? categoryName, string? transactionType)
  {
    var transactions = await _transactionRepository.GetAllTransactionsAsync(userId, page, results, startDate, endDate, categoryName, transactionType);
    return transactions;
  }

  public async Task<TransactionResponseDTO> GetTransactionByIdAsync(string id, string userId)
  {
    var transaction = await _transactionRepository.GetByIdAsync(id, userId);
    var category = await _categoryService.GetCategoryByIdAsync(transaction.CategoryId, userId);

    var response = new TransactionResponseDTO(
           transaction.TransactionId,
           transaction.Amount,
           transaction.TransactionType,
           transaction.CategoryId,
           category.Name,
           transaction.Date,
           transaction.Description,
           transaction.AccountId
       );

    return response;
  }

  public async Task<Summary> GetTransactionSummaryAsync(string userId)
  {
    var transactions = await _transactionRepository.GetAllTransactionsByUserAsync(userId);

    var categories = await _categoryService.GetAllCategoriesAsync(userId);

    var categoryNames = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);

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
  public async Task<bool> RemoveTransactionAsync(string id, string userId)
  {
    var transactionToDelete = await _transactionRepository.GetByIdAsync(id, userId);
    if (transactionToDelete == null)
    {
      return false;
    }
    var amount = transactionToDelete.Amount; 
    var type = transactionToDelete.TransactionType; 
    var accountId = transactionToDelete.AccountId; 

    var account = await _context.Accounts.FindAsync(accountId);
    if (account == null)
    {
      throw new InvalidOperationException($"Associated Account with ID {accountId} not found for transaction {id}.");
    }
    
    var impact = (type.ToLower() == "ingreso") ? amount : -amount;
    account.CurrentBalance -= impact;

    _context.Accounts.Update(account); 

    _context.Transactions.Remove(transactionToDelete); 

    await _context.SaveChangesAsync();

    return true;
  }

  public async Task<TransactionDTO> UpdateTransactionAsync(string id, TransactionDTO transactionDto, string userId)
  {
    var existingTransaction = await _transactionRepository.GetByIdAsync(id, userId);
    if (existingTransaction == null)
    {
      throw new InvalidOperationException($"Transaction with ID {id} not found for transaction {id}.");
    }
    
    var oldAmount = existingTransaction.Amount;
    var oldType = existingTransaction.TransactionType;
    var oldAccountId = existingTransaction.AccountId;
    
    var newAmount = transactionDto.Amount;
    var newType = transactionDto.TransactionType;
    var newAccountId = transactionDto.AccountId;
    
    var oldAccount = await _accountService.GetAccountByIdAsync(oldAccountId, userId);
    if (oldAccount == null)
    {
      throw new InvalidOperationException($"Account with ID {oldAccountId} not found for transaction {id}.");
    }
    
    var oldImpact = (oldType.ToLower() == "ingreso") ? oldAmount : -oldAmount;
    oldAccount.CurrentBalance -= oldImpact;
    
    _transactionRepository.UpdateAync(existingTransaction, transactionDto);

    if (oldAccountId == newAccountId)
    {
      var newImpact = (newType.ToLower() == "ingreso") ? newAmount : -newAmount;
      oldAccount.CurrentBalance += newImpact;
      await _accountService.UpdateAccountAsync(oldAccountId, oldAccount, userId);
    }
    else
    {
      await _accountService.UpdateAccountAsync(oldAccountId, oldAccount, userId);
        
      var newAccount = await _accountService.GetAccountByIdAsync(newAccountId, userId);
      if (newAccount == null)
      {
        throw new InvalidOperationException($"New account with ID {newAccountId} not found.");
      }
        
      var newImpact = (newType.ToLower() == "ingreso") ? newAmount : -newAmount;
      newAccount.CurrentBalance += newImpact;
      await _accountService.UpdateAccountAsync(newAccountId, newAccount, userId);
    }

    await _transactionRepository.SaveChangesAsync();
    
    var updatedTransactionDTO = new TransactionDTO
       (
         existingTransaction.Amount,
         existingTransaction.TransactionType,
         existingTransaction.CategoryId,
         existingTransaction.Date,
         existingTransaction.Description,
         existingTransaction.AccountId
       );

    return updatedTransactionDTO;
  }
}
