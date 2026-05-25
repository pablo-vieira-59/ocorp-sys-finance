using Microsoft.EntityFrameworkCore;
using SysFinance.Domain.Entities;

namespace SysFinance.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Investment> Investments { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<AssetHistory> AssetHistories { get; set; }
    public DbSet<FixedIncomeInvestment> FixedIncomeInvestments { get; set; }
    public DbSet<VariableIncomeInvestment> VariableIncomeInvestments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
