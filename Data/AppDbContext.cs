using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTrackerAPI.Models;

namespace PersonalFinanceTrackerAPI.Data
{
  public class AppDbContext : IdentityDbContext<AppUser>
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Transactions> Transactions { get; set; }
    public DbSet<FinancialGoal> FinancialGoals { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Account> Accounts { get; set; } 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Relación entre Transaction y AppUser
      modelBuilder.Entity<Transactions>()
          .HasOne(t => t.User)
          .WithMany(u => u.Transactions)
          .HasForeignKey(t => t.UserId)
          .OnDelete(DeleteBehavior.Restrict);

      // Relación entre Transaction y Category
      modelBuilder.Entity<Transactions>()
          .HasOne(t => t.Category)
          .WithMany(c => c.Transactions)
          .HasForeignKey(t => t.CategoryId)
          .OnDelete(DeleteBehavior.Restrict);

      // ** 2. AGREGA LA CONFIGURACIÓN DE LA RELACIÓN ACCOUNT y TRANSACTION **
      modelBuilder.Entity<Transactions>() // Configura la entidad Transaction (el lado 'muchos' en esta relación)
          .HasOne(t => t.Account)         // Cada Transaction tiene UNA Account (el lado 'uno')
          .WithMany(a => a.Transactions)  // Una Account puede tener MUCHAS Transactions (necesitas public ICollection<Transaction> Transactions {get;set;} en Account)
          .HasForeignKey(t => t.AccountId)
          .IsRequired(false) // ← ESTO HACE LA FK NULLABLE
          // La clave foránea está en la tabla Transactions y se llama AccountId
          // ** Configura el comportamiento al eliminar una Account **
          // Restrict: Impide eliminar la Account si tiene Transactions. (Suele ser el más seguro)
          // Cascade: Elimina todas las Transactions si se elimina la Account. (Peligroso, fácil perder datos)
          // ClientSetNull: Intenta poner FK a NULL si es nullable. (Requiere que AccountId sea nullable Guid?)
          .OnDelete(DeleteBehavior.Restrict); // O elige otro comportamiento según tu lógica de negocio

      // Relación entre FinancialGoal y Category
      modelBuilder.Entity<FinancialGoal>()
          .HasOne(fg => fg.Category)
          .WithMany(c => c.FinancialGoals)
          .HasForeignKey(fg => fg.CategoryId)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<FinancialGoal>()
          .HasOne(fg => fg.User)
          .WithMany(u => u.FinancialGoals) // Debes agregar esta propiedad en AppUser
          .HasForeignKey(fg => fg.UserId)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Category>()
       .HasOne(c => c.User)
       .WithMany(u => u.Category) // Necesitarás agregar una colección en AppUser
       .HasForeignKey(c => c.UserId)
       .OnDelete(DeleteBehavior.Restrict);
      
      modelBuilder.Entity<Account>(entity =>
      {
          entity.Property(e => e.AccountType)
              .HasConversion<string>() // This is the key line
              .HasColumnType("text"); // Recommended: Specify the column type
      });
    }
  }
}
