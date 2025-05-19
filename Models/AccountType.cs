using System.ComponentModel;

namespace PersonalFinanceTrackerAPI.Models;
public enum AccountType
{
    [Description("Cuenta Corriente")]
    Checking,
    [Description("Cuenta de Ahorro")]
    Savings,
    [Description("Tarjeta de Crédito")]
    CreditCard,
    [Description("Efectivo")]
    Cash,
    [Description("Inversión")]
    Investment,
    [Description("Préstamo")]
    Loan,
    [Description("Cuenta Vista")]
    Vista,
    [Description("Deuda")]
    Deuda
}
