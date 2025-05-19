using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PersonalFinanceTrackerAPI.Interfaces;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[EnableRateLimiting("SlidingWindowPolicy")]
[ApiController]
[Authorize]
public class AccountController:ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponseDTO>> CreateAccount([FromBody] AccountDTO account)
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
            var createdAccount = await _accountService.CreateAccountAsync(account, userId);

            var accountResponse = new AccountResponseDTO
            {
                AccountId = createdAccount.AccountId,
                AccountName = createdAccount.AccountName,
                AccountType = createdAccount.AccountType,
                CurrentBalance = createdAccount.CurrentBalance,
                InitialBalance = createdAccount.InitialBalance
            };
            
            return Ok(accountResponse);
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
    public async Task<ActionResult> DeleteAsync(string id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            await  _accountService.RemoveAccountAsync(id, userId);
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

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDTO>> GetAccountById(string id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var account = await _accountService.GetAccountByIdAsync(id, userId);

            return Ok(account);
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
    public async Task<ActionResult<IEnumerable<AccountResponseDTO>>> GetAllAccountsAsync()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ErrorResponse
                {
                    ErrorCode = "401",
                    ErrorMessage = "No se ha proporcionado un token de autenticación válido o ha expirado. Por favor, inicie sesión nuevamente."
                });
            }
            var accounts = await _accountService.GetAllAccountsAsync(userId);
            return Ok(accounts);
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
    public async Task<ActionResult<AccountResponseDTO>> PutAsync(string id, [FromBody] AccountDTO account)
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

            var updatedAccount = await _accountService.UpdateAccountAsync(id, account, userId);
            return Ok(updatedAccount);
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