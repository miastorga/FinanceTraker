using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class CategoryController : ControllerBase
  {
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
      _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> GetAllCategoriesAsync()
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }
        var categories = await _categoryService.GetAllCategoriesAsync(userId);
        return Ok(categories);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDTO>> GetCategoryById(string id)
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }
        var category = await _categoryService.GetCategoryByIdAsync(id, userId);
        if (id is null)
        {
          return BadRequest();
        }
        if (category == null)
        {
          return NotFound();
        }

        return Ok(category);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponseDTO>> CreateCategory([FromBody] CategoryDTO category)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }
        var createdCategory = await _categoryService.CreateCategoryAsync(category, userId);

        var categoriResponse = new CategoryResponseDTO
        (
          createdCategory.CategoryId,
          createdCategory.Name
        );

        return Ok(categoriResponse);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }
        var category = await _categoryService.RemoveCategoryAsync(id, userId);
        if (category is null)
        {
          return NotFound();
        }
        return NoContent();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryResponseDTO>> PutAsync(string id, [FromBody] CategoryDTO category)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }

        var updatedCategory = await _categoryService.UpdateTransactionAsync(id, category, userId);

        if (updatedCategory is null)
        {
          return NotFound();
        }

        return Ok(updatedCategory);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
