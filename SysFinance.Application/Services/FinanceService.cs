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
    private readonly IAssetHistoryRepository _assetHistoryRepo;

    public FinanceService(
        IRepository<Expense> expenseRepo,
        IRepository<Investment> investmentRepo,
        IRepository<Asset> assetRepo,
        IRepository<Income> incomeRepo,
        IAssetHistoryRepository assetHistoryRepo)
    {
        _expenseRepo = expenseRepo;
        _investmentRepo = investmentRepo;
        _assetRepo = assetRepo;
        _incomeRepo = incomeRepo;
        _assetHistoryRepo = assetHistoryRepo;
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
        return expenses.Select(e => new ExpenseDto(e.Id, e.Description, e.Amount, e.Date, e.Category));
    }

    public async Task<ExpenseDto> AddExpenseAsync(Guid userId, ExpenseDto dto)
    {
        if (dto.Id != null)
        {
            var existingExpense = await _expenseRepo.GetByIdAsync(dto.Id.Value);

            if (existingExpense != null)
            {
                existingExpense.Amount = dto.Amount;
                existingExpense.Date = dto.Date;
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
                Date = dto.Date, 
                Category = dto.Category 
            };

            await _expenseRepo.AddAsync(expense);
            return dto with { Id = expense.Id };
        }
    }

    public async Task<IEnumerable<InvestmentDto>> GetInvestmentsAsync(Guid userId)
    {
        var investments = await _investmentRepo.FindAsync(i => i.UserId == userId);
        return investments.Select(i => new InvestmentDto(i.Id, i.Name, i.Type, i.InvestedAmount, i.CurrentValue, i.MonthlyDividendYield, i.Date));
    }

    public async Task<InvestmentDto> AddInvestmentAsync(Guid userId, InvestmentDto dto)
    {
        var investment = new Investment { UserId = userId, Name = dto.Name, Type = dto.Type, InvestedAmount = dto.InvestedAmount, CurrentValue = dto.CurrentValue, MonthlyDividendYield = dto.MonthlyDividendYield, Date = dto.Date };
        await _investmentRepo.AddAsync(investment);

        if (dto.MonthlyDividendYield > 0)
        {
            var calculatedAmount = dto.CurrentValue * (dto.MonthlyDividendYield / 100.0m);
            var existingIncome = (await _incomeRepo.FindAsync(i => i.UserId == userId && i.Description == dto.Name)).FirstOrDefault();
            if (existingIncome != null)
            {
                existingIncome.Amount = calculatedAmount;
                existingIncome.Type = "Investimentos";
                await _incomeRepo.UpdateAsync(existingIncome);
            }
            else
            {
                var newIncome = new Income
                {
                    UserId = userId,
                    Description = dto.Name,
                    Amount = calculatedAmount,
                    Type = "Investimentos",
                    Discounts = 0
                };
                await _incomeRepo.AddAsync(newIncome);
            }
        }

        return dto with { Id = investment.Id };
    }

    public async Task<InvestmentDto> UpdateInvestmentAsync(Guid userId, Guid investmentId, InvestmentDto dto)
    {
        var investment = (await _investmentRepo.FindAsync(i => i.Id == investmentId && i.UserId == userId)).FirstOrDefault();
        if (investment == null) throw new InvalidOperationException("Investment not found");

        var oldName = investment.Name;

        investment.Name = dto.Name;
        investment.Type = dto.Type;
        investment.InvestedAmount = dto.InvestedAmount;
        investment.CurrentValue = dto.CurrentValue;
        investment.Date = dto.Date;
        investment.MonthlyDividendYield = dto.MonthlyDividendYield;

        await _investmentRepo.UpdateAsync(investment);

        var existingIncome = (await _incomeRepo.FindAsync(i => i.UserId == userId && i.Description == oldName)).FirstOrDefault();

        if (existingIncome != null)
        {
            if (dto.MonthlyDividendYield > 0)
            {
                var calculatedAmount = dto.CurrentValue * (dto.MonthlyDividendYield / 100.0m);
                existingIncome.Description = dto.Name;
                existingIncome.Amount = calculatedAmount;
                existingIncome.Type = "Investimentos";
                await _incomeRepo.UpdateAsync(existingIncome);
            }
            else
            {
                await _incomeRepo.DeleteAsync(existingIncome);
            }
        }
        else if (dto.MonthlyDividendYield > 0)
        {
            var calculatedAmount = dto.CurrentValue * (dto.MonthlyDividendYield / 100.0m);
            var duplicateIncome = (await _incomeRepo.FindAsync(i => i.UserId == userId && i.Description == dto.Name)).FirstOrDefault();
            if (duplicateIncome != null)
            {
                duplicateIncome.Amount = calculatedAmount;
                duplicateIncome.Type = "Investimentos";
                await _incomeRepo.UpdateAsync(duplicateIncome);
            }
            else
            {
                var newIncome = new Income
                {
                    UserId = userId,
                    Description = dto.Name,
                    Amount = calculatedAmount,
                    Type = "Investimentos",
                    Discounts = 0
                };
                await _incomeRepo.AddAsync(newIncome);
            }
        }

        return dto with { Id = investment.Id };
    }

    public async Task DeleteInvestmentAsync(Guid userId, Guid investmentId)
    {
        var investment = (await _investmentRepo.FindAsync(i => i.Id == investmentId && i.UserId == userId)).FirstOrDefault();
        if (investment == null) throw new InvalidOperationException("Investment not found");

        var name = investment.Name;
        await _investmentRepo.DeleteAsync(investment);

        var existingIncome = (await _incomeRepo.FindAsync(i => i.UserId == userId && i.Description == name)).FirstOrDefault();
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
        var totalInvestments = investments.Sum(i => i.CurrentValue);
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
        return assets.Select(a => new IncomeDto(a.Id, a.Description, a.Amount, a.Discounts, a.Type, a.UserId));
    }

    public async Task DeleteIncomeAsync(Guid incomeGuid)
    {
        var existingIncome = await _incomeRepo.GetByIdAsync(incomeGuid);

        if (existingIncome != null)
        {
            var userId = existingIncome.UserId;
            var description = existingIncome.Description;

            await _incomeRepo.DeleteAsync(existingIncome);

            // Find investment with this name for this user and zero its dividend yield rate
            var investment = (await _investmentRepo.FindAsync(i => i.UserId == userId && i.Name == description)).FirstOrDefault();
            if (investment != null)
            {
                investment.MonthlyDividendYield = 0;
                await _investmentRepo.UpdateAsync(investment);
            }
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

            return new IncomeDto(newIncome.Id, newIncome.Description, newIncome.Amount, newIncome.Discounts, newIncome.Type, newIncome.UserId);
        }

    }
}
