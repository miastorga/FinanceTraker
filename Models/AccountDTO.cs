namespace PersonalFinanceTrackerAPI.Models;

public class AccountDTO
{
    public string AccountName { get; set; }
    public AccountType AccountType { get; set; }
    public int CurrentBalance { get; set; }
    public int InitialBalance { get; set; }
}