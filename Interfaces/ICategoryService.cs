using System;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Interfaces;

public interface ICategoryService
{
  Task<Category> CreateCategoryAsync(CategoryDTO categoryDto, string userId);
  Task<CategoryDTO> GetCategoryByIdAsync(string id, string userId);
  Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync(string userId);
  Task<CategoryDTO> RemoveCategoryAsync(string id, string userId);
  Task<CategoryDTO> UpdateTransactionAsync(string id, CategoryDTO categoryDTO, string userId);
}
