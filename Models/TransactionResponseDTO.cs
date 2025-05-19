using System;

namespace PersonalFinanceTrackerAPI.Models;

public record TransactionResponseDTO(
    string TransactionId,
    int Amount,
    string TransactionType,
    string CategoryId,
    string CategoryName,  // Nuevo campo
    DateTime Date,
    string Description,
    string? AccountId
);
