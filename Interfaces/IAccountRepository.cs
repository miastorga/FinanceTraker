using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface IAccountRepository
{
    Task<Account> AddAsync(Account account);
    Task<Account> GetByIdAsync(string id, string userId);
    Task<IEnumerable<AccountResponseDTO>> GetAllAsync(string userId);
    Task<Account> RemoveAync(string id, string userId);
    Task<Account> UpdateAync(string id, AccountDTO accountDTO, string userId);
}