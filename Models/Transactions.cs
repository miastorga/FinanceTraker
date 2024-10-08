using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Transactions;
using Microsoft.AspNetCore.Identity;

namespace PersonalFinanceTrackerAPI.Models;

public class Transactions
{
  [Key] // Esto asegura que se trate como la clave primaria
  public string TransactionId { get; set; }

  [Required(ErrorMessage = "User ID is required.")]
  public string UserId { get; set; }
  // Relación con el usuario
  public AppUser User { get; set; }

  [Required(ErrorMessage = "Amount is required.")]
  [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
  public int Amount { get; set; }

  [Required(ErrorMessage = "Transaction Type is required.")]
  [AllowedValues("ingreso", "gasto")]
  public string TransactionType { get; set; } // Ej. "ingreso" o "gasto"

  [Required(ErrorMessage = "Category ID is required.")]
  public string CategoryId { get; set; }
  [JsonIgnore]
  public Category Category { get; set; }

  [Required(ErrorMessage = "Date is required.")]
  public DateTime Date { get; set; }
  public string Description { get; set; }

}
