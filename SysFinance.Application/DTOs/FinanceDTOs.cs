using SysFinance.Domain.Entities;

namespace SysFinance.Application.DTOs;

public record ExpenseDto(Guid? Id, string Description, decimal Amount, DateTime Date, string Category);
public record InvestmentDto(Guid Id, string Name, string Type, decimal InvestedAmount, decimal CurrentValue, DateTime Date);

public class AssetDto
{
    public Guid? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal EstimatedValue { get; set; }

    public string Type { get; set; } = string.Empty;
}

public class IncomeDto
{
    public IncomeDto(Guid? id, string description, decimal amount, decimal discounts, string type, Guid userId)
    {
        Id = id;
        Description = description;
        Amount = amount;
        Discounts = discounts;
        Type = type;
        UserId = userId;
    }

    public Guid? Id { get; set; }
    public string Description { get; set; } = String.Empty;
    public decimal Amount { get; set; }
    public decimal Discounts { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid UserId { get; set; }


};

public class PatrimonySummaryDto
{
    public decimal TotalInvestments { get; set; }

    public decimal TotalAssets { get; set; }

    public decimal OverallTotal { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }

    public List<InvestmentDto> Investments { get; set; } = new();
    public List<AssetDto> Assets { get; set; } = new();
    public List<AssetHistory> AssetHistories { get; set; } = new();
}