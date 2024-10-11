using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Controllers
{
  [Route("api/[controller]")]
  [EnableRateLimiting("SlidingWindowPolicy")]
  [ApiController]
  [Authorize]
  public class TransactionController : ControllerBase
  {

    private readonly ITransactionService _transactionService;
    public TransactionController(ITransactionService transactionService)
    {
      _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<TransactionResponseDTO>> GetAllTransactionsAsync(
        int page = 1,
        int results = 10,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? categoryName = null,
        string? transactionType = null
        )
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }

        if (startDate.HasValue && startDate.Value.Kind == DateTimeKind.Unspecified)
        {
          startDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
        }

        if (endDate.HasValue && endDate.Value.Kind == DateTimeKind.Unspecified)
        {
          endDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
        }

        var transactions = await _transactionService.GetAllTransactions(userId, page, results, startDate, endDate, categoryName, transactionType);

        return Ok(transactions);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("Summary", Name = "GetTransactionSummary")]
    public async Task<ActionResult<Summary>> GetSummaryAsync()
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }

        var summary = await _transactionService.GetTransactionSummaryAsync(userId);

        return Ok(summary);
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

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDTO>> GetTransactionById(string id)
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }

        var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);

        return Ok(transaction);
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

    [HttpPost]
    public async Task<ActionResult<TransactionResponseDTO>> CreateTransaction([FromBody] TransactionDTO transaction)
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

        var createdTransaction = await _transactionService.CreateTransactionAsync(transaction, userId);
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
    public async Task<ActionResult> RemoveTransactionAsync(string id)
    {
      try
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized();
        }

        var transaction = await _transactionService.RemoveTransactionAsync(id, userId);

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
    public async Task<ActionResult<TransactionResponseDTO>> UpdateTrasanctionAsync(string id, [FromBody] TransactionDTO transactionDTO)
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

        var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transactionDTO, userId);

        return Ok(updatedTransaction);
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

    // Exportar en CSV
  }
}
