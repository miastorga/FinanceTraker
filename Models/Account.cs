using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTrackerAPI.Models;

public class Account
{
    [Key]
    public string AccountId { get; set; }
    [Required(ErrorMessage = "UserId is required.")]
    public string UserId { get; set; } 
    public AppUser User { get; set; }
    
    // Nombre descriptivo de la cuenta (Ej: "Cuenta de Ahorro Principal", "Tarjeta Visa", "Efectivo")
    [Required(ErrorMessage = "Account name is required.")]
    [StringLength(100, ErrorMessage = "The category name can't be longer than 100 characters.")]
    public string AccountName { get; set; }

    // Ejemplos: Checking, Savings, CreditCard, Cash, Investment, Loan (si rastreas deudas)
    [Required(ErrorMessage = "Account Type is required.")]
    [EnumDataType(typeof(AccountType), ErrorMessage = "Not a valid account.")] 
    public AccountType AccountType { get; set; }

    // El saldo actual de esta cuenta
    [Required(ErrorMessage = "Account Type is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Current value must be greater than zero.")]
    public int CurrentBalance { get; set; }

    // Saldo inicial al crear la cuenta (Útil para referencias o ajustes)
    [Range(1, int.MaxValue, ErrorMessage = "Initial value must be greater than zero.")]
    public int InitialBalance { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Transactions> Transactions { get; set; } = new List<Transactions>(); // Inicializar la colección es buena práctica
}
