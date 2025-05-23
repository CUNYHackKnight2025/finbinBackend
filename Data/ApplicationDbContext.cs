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
    public DbSet<UserHistory> UserHistories { get; set; }
    public DbSet<HistorySummary> HistorySummaries { get; set; }

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
            
        modelBuilder.Entity<Expenses>()
            .Property(e => e.RentMortgage)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Expenses>()
            .Property(e => e.Utilities)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Expenses>()
            .Property(e => e.Groceries)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Expenses>()
            .Property(e => e.Transportation)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Expenses>()
            .Property(e => e.Insurance)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Expenses>()
            .Property(e => e.LoanPayments)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Expenses>()
            .Property(e => e.Subscriptions)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Expenses>()
            .Property(e => e.Entertainment)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<FinancialSummary>()
            .Property(f => f.SavingsBalance)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<FinancialSummary>()
            .Property(f => f.InvestmentBalance)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<FinancialSummary>()
            .Property(f => f.DebtBalance)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Income>()
            .Property(i => i.Salary)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Income>()
            .Property(i => i.Investments)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<Income>()
            .Property(i => i.BusinessIncome)
            .HasPrecision(18, 2);
    }
}
