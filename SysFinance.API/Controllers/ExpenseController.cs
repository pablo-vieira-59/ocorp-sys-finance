using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;

namespace SysFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService; // Assuming specialized service exists

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses() => Ok(await _expenseService.GetExpensesAsync(GetUserId()));

    [HttpPost]
    public async Task<IActionResult> AddExpense(ExpenseDto dto) => Ok(await _expenseService.AddExpenseAsync(GetUserId(), dto));

    [HttpDelete("{expenseId}")]
    public async Task<IActionResult> DeleteExpense(Guid expenseId) {
        await _expenseService.DeleteExpenseAsync(expenseId);
        return Ok();
    } 
}