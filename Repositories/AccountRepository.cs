using Microsoft.EntityFrameworkCore;
using PersonalFinanceTrackerAPI.Data;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Repositories;

public class AccountRepository:IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext appContext)
    {
        _context = appContext;
    }
    public async Task<Account> AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<Account> GetByIdAsync(string id, string userId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id && a.UserId == userId);
        if (account is null)
        {
            throw new KeyNotFoundException($"No se encontro la cuenta con el id {id}.");
        }

        return account;
    }

    public async Task<IEnumerable<AccountResponseDTO>> GetAllAsync(string userId)
    {
        var account = await _context.Accounts
            .Where(c => c.UserId == userId)
            .Select(c => new AccountResponseDTO
            {
                AccountId = c.AccountId,
                AccountName = c.AccountName,
                CurrentBalance = c.CurrentBalance,
                InitialBalance = c.InitialBalance,
                AccountType = c.AccountType
            })
            .ToListAsync();

        return account;
    }

    public async Task<Account> RemoveAync(string id, string userId)
    {
        var accountById = await GetByIdAsync(id, userId);
        _context.Accounts.Remove(accountById);
        await _context.SaveChangesAsync();
        return accountById;
    }

    public async Task<Account> UpdateAync(string id, AccountDTO accountDto, string userId)
    {
        var accountById = await GetByIdAsync(id, userId);
        _context.Entry(accountById).CurrentValues.SetValues(accountDto);
        await _context.SaveChangesAsync();
        return accountById;
    }
}