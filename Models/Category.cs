using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PersonalFinanceTrackerAPI.Models;

public class Category
{
  [Key]
  public string CategoryId { get; set; }

  [Required(ErrorMessage = "The category name is required.")]
  [StringLength(100, ErrorMessage = "The category name can't be longer than 100 characters.")]
  public string Name { get; set; }

  [Required(ErrorMessage = "UserId is required.")]
  public string UserId { get; set; }

  [Required]
  public AppUser User { get; set; }
  [JsonIgnore]
  public ICollection<FinancialGoal> FinancialGoals { get; set; }
  [JsonIgnore]
  public ICollection<Transactions> Transactions { get; set; }

}
