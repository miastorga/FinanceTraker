using System;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Services;

public class CategoryService : ICategoryService
{
  private readonly ICategoryRepository _categoryRepository;

  public CategoryService(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }
  public async Task<Category> CreateCategoryAsync(CategoryDTO categoryDto, string userId)
  {

    var category = new Category
    {
      CategoryId = Guid.NewGuid().ToString(),
      UserId = userId,
      Name = categoryDto.Name
    };

    await _categoryRepository.AddAsync(category);
    return category;
  }

  public async Task<CategoryDTO> GetCategoryByIdAsync(string id, string userId)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      throw new ArgumentException("User ID is required.", nameof(id));
    }
    var category = await _categoryRepository.GetByIdAsync(id, userId);

    if (category is null)
    {
      return null;
    }

    var categoryDTO = new CategoryDTO(
      category.Name
    );
    return categoryDTO;
  }

  public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync(string userId)
  {
    return await _categoryRepository.GetAllAsync(userId);
  }

  public async Task<CategoryDTO> RemoveCategoryAsync(string id, string userId)
  {
    var category = await _categoryRepository.RemoveAync(id, userId);
    if (category is null)
    {
      return null;
    }
    var categoryResponse = new CategoryDTO
    (
      category.Name
    );
    return categoryResponse;
  }

  public async Task<CategoryDTO> UpdateTransactionAsync(string id, CategoryDTO categoryDTO, string userId)
  {
    var updatedTransaction = await _categoryRepository.UpdateAync(id, categoryDTO, userId);
    if (updatedTransaction is null)
    {
      return null;
    }
    var updatedTransactionDTO = new CategoryDTO
       (
         updatedTransaction.Name
       );

    return updatedTransactionDTO;
  }
}
