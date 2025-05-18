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

    public Task<IEnumerable<AccountResponseDTO>> GetAllAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Account> RemoveAync(string id, string userId)
    {
        var accountById = await GetByIdAsync(id, userId);
        _context.Accounts.Remove(accountById);
        await _context.SaveChangesAsync();
        return accountById;
    }

    public Task<Account> UpdateAync(string id, CategoryDTO categoryDTO, string userId)
    {
        throw new NotImplementedException();
    }
}