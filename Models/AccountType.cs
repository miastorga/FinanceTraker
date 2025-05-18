namespace PersonalFinanceTrackerAPI.Models;

public enum AccountType
{
    Checking,     // Cuenta Corriente / Vista
    Savings,      // Cuenta de Ahorro
    CreditCard,   // Tarjeta de Crédito
    Cash,         // Efectivo
    Investment,   // Inversión
    Loan          // Préstamo / Deuda (podría ser un tipo de cuenta con saldo negativo)
} 