using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTrackerAPI.Models;

public record TransactionDTO(
  [Required(ErrorMessage = "Amount is required.")]
  [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
  int Amount,

  [Required(ErrorMessage = "Transaction Type is required.")]
  [AllowedValues("ingreso", "gasto")]
  string TransactionType,

  [Required(ErrorMessage = "Category ID is required.")]
  string CategoryId,

  [Required(ErrorMessage = "Date is required.")]
  DateTime Date,

  string Description
  );