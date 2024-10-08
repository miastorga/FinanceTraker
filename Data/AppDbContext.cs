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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Relación entre Transaction y AppUser
      modelBuilder.Entity<Transactions>()
          .HasOne(t => t.User)
          .WithMany(u => u.Transactions)
          .HasForeignKey(t => t.UserId);

      // Relación entre Transaction y Category
      modelBuilder.Entity<Transactions>()
          .HasOne(t => t.Category)
          .WithMany(c => c.Transactions)
          .HasForeignKey(t => t.CategoryId);

      // Relación entre FinancialGoal y Category
      modelBuilder.Entity<FinancialGoal>()
          .HasOne(fg => fg.Category)
          .WithMany(c => c.FinancialGoals)
          .HasForeignKey(fg => fg.CategoryId);

      modelBuilder.Entity<FinancialGoal>()
          .HasOne(fg => fg.User)
          .WithMany(u => u.FinancialGoals) // Debes agregar esta propiedad en AppUser
          .HasForeignKey(fg => fg.UserId);

      modelBuilder.Entity<Category>()
       .HasOne(c => c.User)
       .WithMany(u => u.Category) // Necesitarás agregar una colección en AppUser
       .HasForeignKey(c => c.UserId)
       .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
