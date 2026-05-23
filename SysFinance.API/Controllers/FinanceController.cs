using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;

namespace SysFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public FinanceController(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet("expenses")]
    public async Task<IActionResult> GetExpenses() => Ok(await _financeService.GetExpensesAsync(GetUserId()));
    
    [HttpDelete("expenses/{expenseId}")]
    public async Task<IActionResult> DeleteExpense(Guid expenseId) {
        await _financeService.DeleteExpenseAsync(expenseId);
        return Ok();
    } 
    
    [HttpPost("expenses")]
    public async Task<IActionResult> AddExpense(ExpenseDto dto) => Ok(await _financeService.AddExpenseAsync(GetUserId(), dto));

    [HttpGet("investments")]
    public async Task<IActionResult> GetInvestments() => Ok(await _financeService.GetInvestmentsAsync(GetUserId()));

    [HttpPost("investments")]
    public async Task<IActionResult> AddInvestment(InvestmentDto dto) => Ok(await _financeService.AddInvestmentAsync(GetUserId(), dto));

    [HttpPut("investments/{id}")]
    public async Task<IActionResult> UpdateInvestment(Guid id, InvestmentDto dto) => Ok(await _financeService.UpdateInvestmentAsync(GetUserId(), id, dto));

    [HttpGet("assets")]
    public async Task<IActionResult> GetAssets() => Ok(await _financeService.GetAssetsAsync(GetUserId()));

    [HttpPost("assets")]
    public async Task<IActionResult> AddAsset(AssetDto dto) => Ok(await _financeService.AddAssetAsync(GetUserId(), dto));

    [HttpGet("patrimony")]
    public async Task<IActionResult> GetPatrimony() => Ok(await _financeService.GetPatrimonySummaryAsync(GetUserId()));

    [HttpGet("income")]
    public async Task<IActionResult> GetIncomes() {
        try
        {
            var result = await _financeService.GetIncomesAsync(GetUserId());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost("income")]
    public async Task<IActionResult> AddIncome(IncomeDto dto)
    {
        try
        {
            var user = GetUserId();
            dto.UserId = user;
            var result = await _financeService.AddIncomeAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("income/{incomeId}")]
    public async Task<IActionResult> DeleteIncome(Guid incomeId) { await _financeService.DeleteIncomeAsync(incomeId); return Ok(); } 
}
