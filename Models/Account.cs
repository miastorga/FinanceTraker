namespace PersonalFinanceTrackerAPI.Models;

public class Account
{
    // Identificador único de la cuenta
    public string AccountId { get; set; }

    // Clave foránea para saber a qué usuario pertenece esta cuenta
    public string UserId { get; set; } // Asumiendo que usas string para UserID
    
    // Propiedad de navegación si usas Entity Framework (Opcional pero útil)
    public AppUser User { get; set; }
    
    // Nombre descriptivo de la cuenta (Ej: "Cuenta de Ahorro Principal", "Tarjeta Visa", "Efectivo")
    public string AccountName { get; set; }

    // Tipo de cuenta (Puedes usar un string o, mejor, un Enum)
    // Ejemplos: Checking, Savings, CreditCard, Cash, Investment, Loan (si rastreas deudas)
    public AccountType AccountType { get; set; }

    // El saldo actual de esta cuenta
    public int CurrentBalance { get; set; }

    // Saldo inicial al crear la cuenta (Útil para referencias o ajustes)
    public int InitialBalance { get; set; }

    // Fecha de creación o fecha en que se añadió al sistema
    public DateTime CreatedAt { get; set; }

    // Indicador si la cuenta está activa o si se ha "archivado" (soft delete)
    public bool IsActive { get; set; } = true;
    
    // Lista de transacciones asociadas a esta cuenta (Opcional pero útil)
    public ICollection<Transactions> Transactions { get; set; } = new List<Transactions>(); // Inicializar la colección es buena práctica
}
