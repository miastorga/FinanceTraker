namespace PersonalFinanceTrackerAPI.Models;

public class AccountResponseDTO
{
    public string AccountId { get; set; }

    public string AccountName { get; set; }

    public AccountType AccountType { get; set; }

    public int CurrentBalance { get; set; }

    public int InitialBalance { get; set; }
}