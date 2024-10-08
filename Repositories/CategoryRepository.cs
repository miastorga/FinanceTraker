using System;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTrackerAPI.Data;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Repositories;

public class CategoryRepository : ICategoryRepository
{
  private readonly AppDbContext _context;

  public CategoryRepository(AppDbContext appContext)
  {
    _context = appContext;
  }
  public async Task<Category> AddAsync(Category category)
  {
    await _context.Category.AddAsync(category);
    await _context.SaveChangesAsync();
    return category;
  }

  public async Task<Category> GetByIdAsync(string id, string userId)
  {
    var category = await _context.Category
    .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
    return category;
  }

  public async Task<IEnumerable<CategoryResponseDTO>> GetAllAsync(string userId)
  {
    var category = await _context.Category
      .Where(c => c.UserId == userId)
      .Select(c => new CategoryResponseDTO(c.CategoryId, c.Name))
      .ToListAsync();

    return category;
  }

  public async Task<Category> RemoveAync(string id, string userId)
  {
    var categoryById = await GetByIdAsync(id, userId);
    if (categoryById is null)
    {
      return null;
    }
    _context.Category.Remove(categoryById);
    await _context.SaveChangesAsync();
    return categoryById;
  }

  public async Task<Category> UpdateAync(string id, CategoryDTO categoryDTO, string userId)
  {
    var categoryById = await GetByIdAsync(id, userId);
    if (categoryById is null)
    {
      return null;
    };
    _context.Entry(categoryById).CurrentValues.SetValues(categoryDTO);
    await _context.SaveChangesAsync();

    return categoryById;
  }
}
