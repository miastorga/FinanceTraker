using System;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Migrations;
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
    var category = await _categoryRepository.GetByIdAsync(id, userId);
    var categoryDTO = new CategoryDTO(
      category.Name
    );
    return categoryDTO;
  }

  public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync(string userId)
  {
    var categoria = await _categoryRepository.GetAllAsync(userId);
    return categoria;
  }

  public async Task<CategoryDTO> RemoveCategoryAsync(string id, string userId)
  {

    var category = await _categoryRepository.RemoveAync(id, userId);
    var categoryResponse = new CategoryDTO
    (
      category.Name
    );
    return categoryResponse;
  }

  public async Task<CategoryDTO> UpdateTransactionAsync(string id, CategoryDTO categoryDTO, string userId)
  {
    var updatedCategory = await _categoryRepository.UpdateAync(id, categoryDTO, userId);
    var updatedCategoryDTO = new CategoryDTO
       (
         updatedCategory.Name
       );
    return updatedCategoryDTO;
  }
}
