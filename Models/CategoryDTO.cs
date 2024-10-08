using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTrackerAPI.Models;

public record CategoryDTO(
  [Required(ErrorMessage = "The category name is required.")]
  [StringLength(100, ErrorMessage = "The category name can't be longer than 100 characters.")]
  string Name
  );
