using System;

namespace PersonalFinanceTrackerAPI.Models;

public class ErrorResponse
{
  public string ErrorCode { get; set; } = string.Empty;
  public string ErrorMessage { get; set; } = string.Empty;
}
