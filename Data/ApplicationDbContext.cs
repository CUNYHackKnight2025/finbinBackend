using Microsoft.EntityFrameworkCore;
using BudgetBackend.Models;

namespace BudgetBackend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<BudgetRecord> BudgetRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BudgetRecord>();
        base.OnModelCreating(modelBuilder);
    }
}
