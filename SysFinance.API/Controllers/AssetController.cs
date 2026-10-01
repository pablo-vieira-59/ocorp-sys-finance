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
public class AssetController : ControllerBase
{
    private readonly IAssetService _assetService; // Assuming specialized service exists

    public AssetController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetAssets() => Ok(await _assetService.GetAssetsAsync(GetUserId()));

    [HttpPost]
    public async Task<IActionResult> AddAsset(AssetDto dto) => Ok(await _assetService.AddAssetAsync(GetUserId(), dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsset(Guid id, AssetDto dto) => Ok(await _assetService.UpdateAssetAsync(GetUserId(), id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(Guid id)
    {
        await _assetService.DeleteAssetAsync(GetUserId(), id);
        return Ok();
    }

    [HttpPost("history")]
    public async Task<IActionResult> AddAssetHistory([FromBody] AssetHistoryDto dto)
    {
        dto.UserId = GetUserId();
        await _assetService.AddAssetHistoryAsync(dto);
        return Ok();
    }

    [HttpDelete("history/{id}")]
    public async Task<IActionResult> DeleteAssetHistory(Guid id)
    {
        await _assetService.DeleteAssetHistoryAsync(GetUserId(), id);
        return Ok();
    }

}