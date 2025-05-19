using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTrackerAPI.Models;

public class AccountDTO
{
    [Required(ErrorMessage = "Account name is required.")]
    [StringLength(100, ErrorMessage = "The category name can't be longer than 100 characters.")]
    public string AccountName { get; set; }
    [Required(ErrorMessage = "Account Type is required.")]
    [EnumDataType(typeof(AccountType), ErrorMessage = "Not a valid account.")] 
    public AccountType AccountType { get; set; }
    [Required(ErrorMessage = "Account Type is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Current value must be greater than zero.")]
    public int CurrentBalance { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Initial value must be greater than zero.")]
    public int InitialBalance { get; set; }
}