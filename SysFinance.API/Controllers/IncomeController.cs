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
public class IncomeController : ControllerBase
{
    private readonly IIncomeService _incomeService; // Assuming specialized service exists

    public IncomeController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetIncomes() {
        try
        {
            var result = await _incomeService.GetIncomesAsync(GetUserId());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddIncome(IncomeDto dto)
    {
        try
        {
            var user = GetUserId();
            dto.UserId = user;
            var result = await _incomeService.AddIncomeAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{incomeId}")]
    public async Task<IActionResult> DeleteIncome(Guid incomeId) { await _incomeService.DeleteIncomeAsync(incomeId); return Ok(); } 
}