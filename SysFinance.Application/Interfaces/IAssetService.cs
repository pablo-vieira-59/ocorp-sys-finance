using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using SysFinance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysFinance.Application.Interfaces;

public interface IAssetService
{
    Task<IEnumerable<AssetDto>> GetAssetsAsync(Guid userId);
    Task<AssetDto> AddAssetAsync(Guid userId, AssetDto assetDto);
    Task<AssetDto> UpdateAssetAsync(Guid userId, Guid assetId, AssetDto assetDto);
    Task DeleteAssetAsync(Guid userId, Guid assetId);

    Task AddAssetHistoryAsync(AssetHistoryDto dto);
    Task DeleteAssetHistoryAsync(Guid userId, Guid historyId);
    Task<IEnumerable<AssetHistory>> GetAssetHistoryAsync(Guid userId);
}