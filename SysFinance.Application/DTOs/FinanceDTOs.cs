using SysFinance.Domain.Entities;

namespace SysFinance.Application.DTOs;

public record ExpenseDto(Guid? Id, string Description, decimal Amount, DateTime Date, string Category);
public record InvestmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal InvestedAmount { get; set; }
    public decimal CurrentValue { get; set; }
    
    private decimal _monthlyDividendYield;
    public decimal MonthlyDividendYield 
    { 
        get => _monthlyDividendYield; 
        set => _monthlyDividendYield = value; 
    }
    
    public decimal MonthlyDividendYeild 
    { 
        get => _monthlyDividendYield; 
        set => _monthlyDividendYield = value; 
    }
    
    public DateTime Date { get; set; }

    public InvestmentDto() { }

    public InvestmentDto(Guid id, string name, string type, decimal investedAmount, decimal currentValue, decimal monthlyDividendYield, DateTime date)
    {
        Id = id;
        Name = name;
        Type = type;
        InvestedAmount = investedAmount;
        CurrentValue = currentValue;
        MonthlyDividendYield = monthlyDividendYield;
        Date = date;
    }

    public void Deconstruct(out Guid id, out string name, out string type, out decimal investedAmount, out decimal currentValue, out decimal monthlyDividendYield, out DateTime date)
    {
        id = Id;
        name = Name;
        type = Type;
        investedAmount = InvestedAmount;
        currentValue = CurrentValue;
        monthlyDividendYield = MonthlyDividendYield;
        date = Date;
    }
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

public class AssetHistoryInputDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}