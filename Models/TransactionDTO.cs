using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTrackerAPI.Models;

public record TransactionDTO(
  int Amount,

  string TransactionType,

  string CategoryId,

  DateTime Date,

  string Description,
  
  string? AccountId
  );