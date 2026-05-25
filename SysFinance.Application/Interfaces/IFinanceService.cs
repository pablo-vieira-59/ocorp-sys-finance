using SysFinance.Application.DTOs;
using SysFinance.Domain.Entities;

namespace SysFinance.Application.Interfaces;

public interface IFinanceService
{
    Task DeleteExpenseAsync(Guid expenseId);
    Task<IEnumerable<ExpenseDto>> GetExpensesAsync(Guid userId);
    Task<ExpenseDto> AddExpenseAsync(Guid userId, ExpenseDto expenseDto);
    
    Task<IEnumerable<InvestmentDto>> GetInvestmentsAsync(Guid userId);
    Task<InvestmentDto> AddInvestmentAsync(Guid userId, InvestmentDto investmentDto);
    Task DeleteInvestmentAsync(Guid userId, Guid investmentId);

    Task<IEnumerable<AssetDto>> GetAssetsAsync(Guid userId);
    Task<AssetDto> AddAssetAsync(Guid userId, AssetDto assetDto);
    Task<AssetDto> UpdateAssetAsync(Guid userId, Guid assetId, AssetDto assetDto);
    Task DeleteAssetAsync(Guid userId, Guid assetId);

    Task AddAssetHistoryAsync(AssetHistoryDto dto);
    Task DeleteAssetHistoryAsync(Guid userId, Guid historyId);
    
    Task<PatrimonySummaryDto> GetPatrimonySummaryAsync(Guid userId);

    Task<IEnumerable<IncomeDto>> GetIncomesAsync(Guid userId);
    Task<IncomeDto> AddIncomeAsync(IncomeDto incomeDto);
    Task DeleteIncomeAsync(Guid incomeGuid);

    Task<List<AssetHistory>> GetAssetHistoryAsync(Guid userId);
}
