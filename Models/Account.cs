namespace PersonalFinanceTrackerAPI.Models;

public class Account
{
    public string AccountId { get; set; }
    public string UserId { get; set; } 
    public AppUser User { get; set; }
    
    // Nombre descriptivo de la cuenta (Ej: "Cuenta de Ahorro Principal", "Tarjeta Visa", "Efectivo")
    public string AccountName { get; set; }

    // Ejemplos: Checking, Savings, CreditCard, Cash, Investment, Loan (si rastreas deudas)
    public AccountType AccountType { get; set; }

    // El saldo actual de esta cuenta
    public int CurrentBalance { get; set; }

    // Saldo inicial al crear la cuenta (Útil para referencias o ajustes)
    public int InitialBalance { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; } = true;
    public ICollection<Transactions> Transactions { get; set; } = new List<Transactions>(); // Inicializar la colección es buena práctica
}
