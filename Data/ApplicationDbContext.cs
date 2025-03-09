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
    public DbSet<Bucket> Buckets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

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

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.UserId);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.Category);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.TransactionDate);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);
    }
}
