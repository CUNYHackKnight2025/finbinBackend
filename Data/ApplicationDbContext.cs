/*
Were going to use EF core to query and manipulate data without writing any raw SQL queries
So we want to add the bridge between our objects and our db
*/

using Microsoft.EntityFrameworkCore;
using BudgetBackend.Models;

namespace BudgetBackend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<BudgetRecord> BudgetRecords { get; set; }
}
