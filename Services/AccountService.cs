using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Services;

public class AccountService:IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository categoryRepository)
    {
        _accountRepository = categoryRepository;
    }

    public async Task<Account> CreateAccountAsync(AccountDTO accountDTO, string userId)
    {
        var account = new Account
        {
            AccountId = Guid.NewGuid().ToString(),
            UserId = userId,
            AccountName = accountDTO.AccountName,
            AccountType = accountDTO.AccountType,
            CurrentBalance = accountDTO.CurrentBalance,
            InitialBalance = accountDTO.InitialBalance,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
       await _accountRepository.AddAsync(account);
       return account;
    }

    public async Task<AccountDTO> GetAccountByIdAsync(string id, string userId)
    {
        var account = await _accountRepository.GetByIdAsync(id, userId);
        var accountResponse = new AccountDTO
        {
            AccountName = account.AccountName,
            AccountType = account.AccountType,
            CurrentBalance = account.CurrentBalance,
            InitialBalance = account.InitialBalance
        };
        return accountResponse;
    }

    public async Task<IEnumerable<AccountResponseDTO>> GetAllAccountsAsync(string userId)
    {
        var accounts = await _accountRepository.GetAllAsync(userId);
        return accounts;
    }
    

    public async Task<AccountDTO> RemoveAccountAsync(string id, string userId)
    {
        var account = await _accountRepository.RemoveAync(id, userId);
        var accountResponse = new AccountDTO
        {
            AccountName = account.AccountName,
            AccountType = account.AccountType,
            CurrentBalance = account.CurrentBalance,
            InitialBalance = account.InitialBalance
        };
        return accountResponse;
    }

    public Task<AccountDTO> UpdateAccountAsync(string id, AccountDTO accountDTO, string userId)
    {
        throw new NotImplementedException();
    }
}