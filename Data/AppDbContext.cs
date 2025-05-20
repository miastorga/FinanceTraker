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

    // Configuración explícita para AccountId nullable
    modelBuilder.Entity<Transactions>(entity =>
    {
        entity.Property(t => t.AccountId)
            .IsRequired(false); // Esto hace que la columna sea nullable
    });

    modelBuilder.Entity<Transactions>()
        .HasOne(t => t.User)
        .WithMany(u => u.Transactions)
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Transactions>()
        .HasOne(t => t.Category)
        .WithMany(c => c.Transactions)
        .HasForeignKey(t => t.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    // Configuración más específica para AccountId nullable
    modelBuilder.Entity<Transactions>(entity =>
    {
        entity.Property(t => t.AccountId)
            .IsRequired(false); // Asegura que la propiedad sea nullable
        
        entity.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .IsRequired(false) // La relación es opcional
            .OnDelete(DeleteBehavior.SetNull); // Cambiado a SetNull para manejar mejor los nulls
    }); 

    // Resto de configuraciones...
    modelBuilder.Entity<FinancialGoal>()
        .HasOne(fg => fg.Category)
        .WithMany(c => c.FinancialGoals)
        .HasForeignKey(fg => fg.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<FinancialGoal>()
        .HasOne(fg => fg.User)
        .WithMany(u => u.FinancialGoals) 
        .HasForeignKey(fg => fg.UserId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Category>()
     .HasOne(c => c.User)
     .WithMany(u => u.Category) 
     .HasForeignKey(c => c.UserId)
     .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<Account>(entity =>
    {
        entity.Property(e => e.AccountType)
            .HasConversion<string>() 
            .HasColumnType("text"); 
    });
}
  }
}
