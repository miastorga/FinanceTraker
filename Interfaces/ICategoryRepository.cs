using System;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface ICategoryRepository
{
  Task<Category> AddAsync(Category category);
  Task<Category> GetByIdAsync(string id, string userId);
  Task<IEnumerable<CategoryResponseDTO>> GetAllAsync(string userId);
  Task<Category> RemoveAync(string id, string userId);
  Task<Category> UpdateAync(string id, CategoryDTO categoryDTO, string userId);
}
