namespace SysFinance.Domain.Entities;

public class Investment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal InvestedAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal MonthlyDividendYield { get; set; }
    public DateTime Date { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
}
