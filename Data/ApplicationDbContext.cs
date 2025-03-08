using Microsoft.EntityFrameworkCore;
using BudgetBackend.Models;

namespace BudgetBackend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<FinancialSummary> FinancialSummaries { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Expenses> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FinancialSummary>()
            .HasOne(f => f.Income)
            .WithOne(i => i.FinancialSummary)
            .HasForeignKey<Income>(i => i.FinancialSummaryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FinancialSummary>()
            .HasOne(f => f.Expenses)
            .WithOne(e => e.FinancialSummary)
            .HasForeignKey<Expenses>(e => e.FinancialSummaryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
