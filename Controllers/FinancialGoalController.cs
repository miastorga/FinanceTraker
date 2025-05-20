using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.VisualBasic;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Controllers
{
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiVersion("1.0")]
  [EnableRateLimiting("SlidingWindowPolicy")]
  [ApiController]
  public class FinancialGoalController : ControllerBase
  {
    private readonly IFinancialGoalService _financialGoalService;

    public FinancialGoalController(IFinancialGoalService financialGoalService)
    {
      _financialGoalService = financialGoalService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FinancialGoalDTO>> GetFinancialGoalById(string id)
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }

        var financialGoal = await _financialGoalService.GetFinancialGoalByIdAsync(id, userId);
        return Ok(financialGoal);
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(new ErrorResponse
        {
          ErrorCode = "404",
          ErrorMessage = ex.Message
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new ErrorResponse
        {
          ErrorCode = "400",
          ErrorMessage = ex.Message
        });
      }
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<FinancialGoalDTO>>> GetAllFinancialGoals(
      DateTime? startDate, DateTime? endDate, string? categoryName, string? period, int goalAmount = 0,
      int page = 1, int results = 10
      )
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }

        if (page <= 0)
        {
          page = 1;
        }
        
        // Si results es <= 0, usamos el valor predeterminado 10
        if (results <= 0)
        {
          results = 10;
        }
        
        if (startDate.HasValue && startDate.Value.Kind == DateTimeKind.Unspecified)
        {
          startDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
        }

        if (endDate.HasValue && endDate.Value.Kind == DateTimeKind.Unspecified)
        {
          endDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
        }

        var financialGoal = await _financialGoalService.GetAllFinancialGoals(page, results, startDate, endDate, categoryName, period, goalAmount, userId);

        return Ok(financialGoal);
      }
      catch (Exception ex)
      {
        return BadRequest(new ErrorResponse
        {
          ErrorCode = "400",
          ErrorMessage = ex.Message
        });
      }
    }
    [HttpPost]
    public async Task<ActionResult<FinancialGoalDTO>> CreateFinancialGoal([FromBody] FinancialGoalDTO financialGoalDTO)
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

        var createdTransaction = await _financialGoalService.CreateFinancialGoalAsync(financialGoalDTO, userId);
        return Ok(createdTransaction);
      }
      catch (Exception ex)
      {
        return BadRequest(new ErrorResponse
        {
          ErrorCode = "400",
          ErrorMessage = ex.Message
        });
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveFinancialGoalAsync(string id)
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }

        var financialGoal = await _financialGoalService.RemoveFinancialGoalAsync(id, userId);
        return NoContent();
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(new ErrorResponse
        {
          ErrorCode = "404",
          ErrorMessage = ex.Message
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new ErrorResponse
        {
          ErrorCode = "400",
          ErrorMessage = ex.Message
        });
      }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FinancialGoalDTO>> UpdateFinancialGoalAsync(string id, [FromBody] FinancialGoalDTO financialGoalDTO)
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

        var updatedFinancialGoal = await _financialGoalService.UpdateFinancialGoalAsync(id, financialGoalDTO, userId);
        return Ok(updatedFinancialGoal);
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(new ErrorResponse
        {
          ErrorCode = "404",
          ErrorMessage = ex.Message
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new ErrorResponse
        {
          ErrorCode = "400",
          ErrorMessage = ex.Message
        });
      }
    }
  }
}
