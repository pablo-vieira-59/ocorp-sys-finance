using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using SysFinance.Domain.Entities;
using SysFinance.Domain.Interfaces;

namespace SysFinance.Application.Services;

public class FinanceService : IFinanceService
{
    private readonly IRepository<Expense> _expenseRepo;
    private readonly IRepository<Investment> _investmentRepo;
    private readonly IRepository<Asset> _assetRepo;
    private readonly IRepository<Income> _incomeRepo;
    private readonly IRepository<FixedIncomeInvestment> _fixedIncomeInvestmentRepo;
    private readonly IRepository<VariableIncomeInvestment> _variableIncomeInvestmentRepo;
    private readonly IAssetHistoryRepository _assetHistoryRepo;

    public FinanceService(
        IRepository<Expense> expenseRepo,
        IRepository<Investment> investmentRepo,
        IRepository<Asset> assetRepo,
        IRepository<Income> incomeRepo,
        IRepository<FixedIncomeInvestment> fixedIncomeInvestmentRepo,
        IRepository<VariableIncomeInvestment> variableIncomeInvestmentRepo,
        IAssetHistoryRepository assetHistoryRepo)
    {
        _expenseRepo = expenseRepo;
        _investmentRepo = investmentRepo;
        _assetRepo = assetRepo;
        _incomeRepo = incomeRepo;
        _assetHistoryRepo = assetHistoryRepo;
        _fixedIncomeInvestmentRepo = fixedIncomeInvestmentRepo;
        _variableIncomeInvestmentRepo = variableIncomeInvestmentRepo;
    }

    public async Task DeleteExpenseAsync(Guid expenseId)
    {
        var existingExpense = await _expenseRepo.GetByIdAsync(expenseId);
        if (existingExpense != null)
        {
            await _expenseRepo.DeleteAsync(existingExpense);
        }
    }

    public async Task<IEnumerable<ExpenseDto>> GetExpensesAsync(Guid userId)
    {
        var expenses = await _expenseRepo.FindAsync(e => e.UserId == userId);
        return expenses.Select(e => new ExpenseDto{
            Id = e.Id,
            Description = e.Description,
            Amount = e.Amount,
            Category = e.Category 
        });
    }

    public async Task<ExpenseDto> AddExpenseAsync(Guid userId, ExpenseDto dto)
    {
        if (dto.Id != null)
        {
            var existingExpense = await _expenseRepo.GetByIdAsync(dto.Id.Value);

            if (existingExpense != null)
            {
                existingExpense.Amount = dto.Amount;
                existingExpense.Category = dto.Category;
                existingExpense.Description = dto.Description;

                await _expenseRepo.UpdateAsync(existingExpense);
            }
            return dto;
        }
        else
        {
            var expense = new Expense { 
                UserId = userId, 
                Description = dto.Description, 
                Amount = dto.Amount, 
                Category = dto.Category 
            };

            await _expenseRepo.AddAsync(expense);
            return dto with { Id = expense.Id };
        }
    }

    public async Task<IEnumerable<InvestmentDto>> GetInvestmentsAsync(Guid userId)
    {
        var investments = await _investmentRepo.FindAsync(i => i.UserId == userId,i => i.Variable,i => i.Fixed);
        return investments.Select(i => new InvestmentDto
        {
            Id = i.Id,
            Name = i.Name,
            Type = i.Type,
            CreatedAt = i.CreatedAt,
            Fixed = i.Fixed == null ? null : new FixedIncomeInvestmentDto
            {
                CurrentAmount = i.Fixed.CurrentAmount,
                Id = i.Id,
                InitialAmount = i.Fixed.InitialAmount,
                InterestRate = i.Fixed.InterestRate,
                InvestmentId = i.Fixed.InvestmentId,
            },
            Variable = i.Variable == null ? null : new VariableIncomeInvestmentDto
            {
                AveragePrice = i.Variable.AveragePrice,
                Id = i.Id,
                CurrentQuotePrice = i.Variable.CurrentQuotePrice,
                InvestedAmount = i.Variable.InvestedAmount,
                InvestmentId = i.Variable.InvestmentId,
                MonthlyDividendYield = i.Variable.MonthlyDividendYield,
                Quantity = i.Variable.Quantity,
            },
        });
    }

    public async Task<InvestmentDto> AddInvestmentAsync(Guid userId, InvestmentDto dto)
    {
        if(dto.Id == null)
        {
            var investment = new Investment
            {
                UserId = userId,
                Name = dto.Name,
                Type = dto.Type,
                CreatedAt = dto.CreatedAt
            };

            if(dto.Type == "Renda Fixa")
            {
                investment.Fixed = new FixedIncomeInvestment
                {
                    CurrentAmount = dto.Fixed!.CurrentAmount,
                    InitialAmount = dto.Fixed!.InitialAmount,
                    InterestRate = dto.Fixed!.InterestRate,
                };

                if (investment.Fixed.InterestRate > 0)
                {
                    var income = new Income
                    {
                        Amount = dto.Fixed!.CurrentAmount * ((dto.Fixed!.InterestRate / 100) / 12),
                        Description = dto.Name,
                        Discounts = 0,
                        UserId = userId,
                        Type = "Investimento"
                    };

                    await _incomeRepo.AddAsync(income);
                }
            }
            else
            {
                investment.Variable = new VariableIncomeInvestment
                {
                    CurrentQuotePrice = dto.Variable!.CurrentQuotePrice,
                    AveragePrice = dto.Variable!.AveragePrice,
                    InvestedAmount = dto.Variable!.InvestedAmount,
                    MonthlyDividendYield = dto.Variable!.MonthlyDividendYield,
                    Quantity = dto.Variable!.Quantity
                };

                if (investment.Variable.MonthlyDividendYield > 0)
                {
                    var income = new Income
                    {
                        Amount = (dto.Variable!.CurrentQuotePrice * (decimal)dto.Variable!.Quantity) * (dto.Variable!.MonthlyDividendYield / 100),
                        Description = dto.Name,
                        Discounts = 0,
                        UserId = userId,
                        Type = "Investimento"
                    };

                    await _incomeRepo.AddAsync(income);
                }
            }

            await _investmentRepo.AddAsync(investment);

            return new InvestmentDto { Id = investment.Id };
        }
        else
        {
            var existingInvestments = await _investmentRepo.FindAsync(x => x.Id == dto.Id.Value,x=> x.Variable,x=>x.Fixed);
            var existingInvestment = existingInvestments.FirstOrDefault();

            if (existingInvestment != null) 
            {
                existingInvestment.Name = dto.Name;
                existingInvestment.Type = dto.Type;

                if(existingInvestment.Type == "Renda Fixa" && existingInvestment.Fixed == null)
                {
                    existingInvestment.Fixed = new FixedIncomeInvestment
                    {
                        CurrentAmount = dto.Fixed.CurrentAmount,
                        InterestRate = dto.Fixed.InterestRate,
                        InitialAmount = dto.Fixed.InitialAmount,
                        InvestmentId = dto.Fixed.InvestmentId
                    };

                    await _fixedIncomeInvestmentRepo.AddAsync(existingInvestment.Fixed);
                }

                if (existingInvestment.Type != "Renda Fixa" && existingInvestment.Variable == null)
                {
                    existingInvestment.Variable = new VariableIncomeInvestment
                    {
                        AveragePrice = dto.Variable.AveragePrice,
                        Quantity = dto.Variable.Quantity,
                        InvestedAmount = dto.Variable.InvestedAmount,
                        InvestmentId = dto.Variable.InvestmentId,
                        CurrentQuotePrice = dto.Variable.CurrentQuotePrice,
                        MonthlyDividendYield = dto.Variable.MonthlyDividendYield,
                    };

                    await _variableIncomeInvestmentRepo.AddAsync(existingInvestment.Variable);
                }

                if (existingInvestment.Fixed != null)
                {
                    existingInvestment.Fixed.InterestRate = dto.Fixed.InterestRate;
                    existingInvestment.Fixed.CurrentAmount = dto.Fixed.CurrentAmount;
                    existingInvestment.Fixed.InitialAmount = dto.Fixed.InitialAmount;
                }

                if(existingInvestment.Variable != null)
                {
                    existingInvestment.Variable.MonthlyDividendYield = dto.Variable.MonthlyDividendYield;
                    existingInvestment.Variable.Quantity = dto.Variable.Quantity;
                    existingInvestment.Variable.CurrentQuotePrice = dto.Variable.CurrentQuotePrice;
                    existingInvestment.Variable.AveragePrice = dto.Variable.AveragePrice;
                    existingInvestment.Variable.InvestedAmount = dto.Variable.InvestedAmount;
                }

                await _investmentRepo.UpdateAsync(existingInvestment);

                var income = (await _incomeRepo.FindAsync(x => x.UserId == existingInvestment.UserId 
                && x.Description == existingInvestment.Name 
                && x.Type == "Investimento")).FirstOrDefault();

                if (income != null) 
                { 
                    if(existingInvestment.Type == "Renda Fixa")
                    {
                        income.Amount = dto.Fixed!.CurrentAmount * ((dto.Fixed!.InterestRate / 100) / 12);
                        await _incomeRepo.UpdateAsync(income);
                    }
                    else
                    {
                        income.Amount = (dto.Variable!.CurrentQuotePrice * (decimal)dto.Variable!.Quantity) * (dto.Variable!.MonthlyDividendYield / 100);
                        if(dto.Variable!.MonthlyDividendYield <= 0)
                        {
                            await _incomeRepo.DeleteAsync(income);
                        }
                        else
                        {
                            await _incomeRepo.UpdateAsync(income);
                        }
                    }

                }
            }

            return new InvestmentDto { Id = dto.Id };
        }

    }

    public async Task DeleteInvestmentAsync(Guid userId, Guid investmentId)
    {
        var investment = (await _investmentRepo.FindAsync(i => i.Id == investmentId && i.UserId == userId)).FirstOrDefault();
        if (investment == null) throw new InvalidOperationException("Investment not found");

        var name = investment.Name;
        await _investmentRepo.DeleteAsync(investment);

        var existingIncome = (await _incomeRepo.FindAsync(i => i.UserId == userId && i.Description == name && i.Type == "Investimento")).FirstOrDefault();
        if (existingIncome != null)
        {
            await _incomeRepo.DeleteAsync(existingIncome);
        }
    }

    public async Task<IEnumerable<AssetDto>> GetAssetsAsync(Guid userId)
    {
        var assets = await _assetRepo.FindAsync(a => a.UserId == userId);
        return assets.Select(a => new AssetDto{
            Id = a.Id, 
            Name = a.Name,
            Description = a.Description,
            EstimatedValue = a.EstimatedValue,
            Type = a.Type 
        });
    }

    public async Task<AssetDto> AddAssetAsync(Guid userId, AssetDto dto)
    {
        var asset = new Asset { UserId = userId, Name = dto.Name, Description = dto.Description, EstimatedValue = dto.EstimatedValue, Type = dto.Type };
        await _assetRepo.AddAsync(asset);
        return new AssetDto { Id = asset.Id};
    }

    public async Task<AssetDto> UpdateAssetAsync(Guid userId, Guid assetId, AssetDto dto)
    {
        var asset = (await _assetRepo.FindAsync(a => a.Id == assetId && a.UserId == userId)).FirstOrDefault();
        if (asset == null) throw new InvalidOperationException("Asset not found");

        asset.Name = dto.Name;
        asset.Description = dto.Description;
        asset.EstimatedValue = dto.EstimatedValue;
        asset.Type = dto.Type;

        await _assetRepo.UpdateAsync(asset);
        return new AssetDto { Id = asset.Id, Name = asset.Name, Description = asset.Description, EstimatedValue = asset.EstimatedValue, Type = asset.Type };
    }

    public async Task DeleteAssetAsync(Guid userId, Guid assetId)
    {
        var asset = (await _assetRepo.FindAsync(a => a.Id == assetId && a.UserId == userId)).FirstOrDefault();
        if (asset == null) throw new InvalidOperationException("Asset not found");
        await _assetRepo.DeleteAsync(asset);
    }

    public async Task AddAssetHistoryAsync(AssetHistoryDto dto)
    {
        if(dto.Id == null)
        {
            var history = new AssetHistory { UserId = dto.UserId, Date = dto.Date, Amount = dto.Amount };
            await _assetHistoryRepo.AddAsync(history);
        }
        else
        {
            var existingHistory = await _assetHistoryRepo.GetByIdAsync(dto.Id.Value);

            if (existingHistory != null)
            {
                existingHistory.Date = dto.Date;
                existingHistory.Amount = dto.Amount;

                await _assetHistoryRepo.UpdateAsync(existingHistory);
            }
        }
    }

    public async Task DeleteAssetHistoryAsync(Guid userId, Guid historyId)
    {
        var history = await _assetHistoryRepo.GetByIdAsync(historyId);
        if (history == null || history.UserId != userId) throw new InvalidOperationException("History not found");
        await _assetHistoryRepo.DeleteAsync(history);
    }

    public async Task<List<AssetHistory>> GetAssetHistoryAsync(Guid userId)
    {
        var histories = await _assetHistoryRepo.GetAllByUserId(userId);
        return histories;
    }

    public async Task<PatrimonySummaryDto> GetPatrimonySummaryAsync(Guid userId)
    {
        var investments = await GetInvestmentsAsync(userId);
        var assets = await GetAssetsAsync(userId);
        var incomes = await GetIncomesAsync(userId);
        var expenses = await GetExpensesAsync(userId);
        var history = await GetAssetHistoryAsync(userId);
        
        var totalIncome = incomes.Sum(x => x.Amount);
        var totalExpenses = expenses.Sum(x => x.Amount);
        var totalInvestments = investments.Sum(i => 
            (i.Type == "Renda Fixa" ? i.Fixed.CurrentAmount : ((decimal)i.Variable.Quantity * i.Variable.CurrentQuotePrice))
        );
        var totalAssets = assets.Sum(a => a.EstimatedValue);
        var overallTotal = totalInvestments + totalAssets;

        return new PatrimonySummaryDto
        {
            AssetHistories = history,
            Investments = investments.ToList(),
            Assets = assets.ToList(),
            TotalInvestments = totalInvestments,
            TotalAssets = totalAssets,
            OverallTotal = overallTotal,
            TotalExpenses = totalExpenses,
            TotalIncome = totalIncome,
        };
    }
    
    public async Task<IEnumerable<IncomeDto>> GetIncomesAsync(Guid userId)
    {
        var assets = await _incomeRepo.FindAsync(a => a.UserId == userId);
        return assets.Select(a => new IncomeDto { 
            Id = a.Id, 
            Description = a.Description,
            Amount = a.Amount,
            Discounts = a.Discounts,
            Type = a.Type,
            UserId = a.UserId 
        });
    }

    public async Task DeleteIncomeAsync(Guid incomeGuid)
    {
        var existingIncome = await _incomeRepo.GetByIdAsync(incomeGuid);

        if (existingIncome != null)
        {
            var userId = existingIncome.UserId;
            var description = existingIncome.Description;

            if (existingIncome!.Type == "Investimento")
            {
                var investment = (await _investmentRepo.FindAsync(x => x.UserId == userId
                && x.Name == existingIncome.Description)).FirstOrDefault();

                if (investment != null)
                {
                    await _investmentRepo.DeleteAsync(investment);
                }
            }

            await _incomeRepo.DeleteAsync(existingIncome);
        }
    }

    public async Task<IncomeDto> AddIncomeAsync(IncomeDto incomeDto)
    {
        if(incomeDto.Id != null)
        {
            var existingIncome = await _incomeRepo.GetByIdAsync(incomeDto.Id.Value);

            if(existingIncome != null)
            {
                existingIncome.Description = incomeDto.Description;
                existingIncome.Amount = incomeDto.Amount;
                existingIncome.Discounts = incomeDto.Discounts;
                existingIncome.Type = incomeDto.Type;

                await _incomeRepo.UpdateAsync(existingIncome);
            }

            return incomeDto;
        }
        else
        {
            var newIncome = new Income
            {
                Amount = incomeDto.Amount,
                UserId = incomeDto.UserId,
                Description = incomeDto.Description,
                Discounts = incomeDto.Discounts,
                Type = incomeDto.Type,
                Id = Guid.NewGuid(),
            };

            await _incomeRepo.AddAsync(newIncome);

            return new IncomeDto { 
                Id = newIncome.Id, 
                Description = newIncome.Description,
                Amount = newIncome.Amount,
                Discounts = newIncome.Discounts,
                Type = newIncome.Type,
                UserId = newIncome.UserId 
            };
        }

    }
}
