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
}