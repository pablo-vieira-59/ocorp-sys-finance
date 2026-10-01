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
public class InvestmentController : ControllerBase
{
    private readonly IInvestmentService _investmentService; // Assuming specialized service exists

    public InvestmentController(IInvestmentService investmentService)
    {
        _investmentService = investmentService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetInvestments() => Ok(await _investmentService.GetInvestmentsAsync(GetUserId()));

    [HttpPost]
    public async Task<IActionResult> AddInvestment(InvestmentDto dto) => Ok(await _investmentService.AddInvestmentAsync(GetUserId(), dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvestment(Guid id)
    {
        await _investmentService.DeleteInvestmentAsync(GetUserId(), id);
        return Ok();
    }
}