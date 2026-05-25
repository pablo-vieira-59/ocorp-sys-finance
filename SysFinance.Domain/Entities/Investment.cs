namespace SysFinance.Domain.Entities;

public class Investment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
    public FixedIncomeInvestment? Fixed { get; set; }
    public VariableIncomeInvestment? Variable { get; set; }
}
