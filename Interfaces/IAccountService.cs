using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface IAccountService
{
    Task<Account> CreateAccountAsync(AccountDTO accountDTO, string userId);
    Task<AccountDTO> GetAccountByIdAsync(string id, string userId);
    Task<IEnumerable<AccountResponseDTO>> GetAllAccountsAsync(string userId);
    Task<AccountDTO> RemoveAccountAsync(string id, string userId);
    Task<AccountDTO> UpdateAccountAsync(string id, AccountDTO accountDTO, string userId);
}