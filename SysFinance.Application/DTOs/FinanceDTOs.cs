using SysFinance.Domain.Entities;

namespace SysFinance.Application.DTOs;

public record ExpenseDto
{
    public Guid? Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class InvestmentDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; 
    public DateTime CreatedAt { get; set; }
}

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

public class AssetHistoryDto
{
    public Guid? Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}