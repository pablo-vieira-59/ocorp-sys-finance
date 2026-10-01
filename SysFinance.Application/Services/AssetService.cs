using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using SysFinance.Domain.Entities;
using SysFinance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysFinance.Application.Services;

public class AssetService : IAssetService
{
    private readonly IRepository<Asset> _assetRepo;
    private readonly IAssetHistoryRepository _assetHistoryRepo;

    public AssetService(IRepository<Asset> assetRepo, IAssetHistoryRepository assetHistoryRepo)
    {
        _assetRepo = assetRepo;
        _assetHistoryRepo = assetHistoryRepo;
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

    public async Task<IEnumerable<AssetHistory>> GetAssetHistoryAsync(Guid userId)
    {
        var histories = await _assetHistoryRepo.GetAllByUserId(userId);
        return histories;
    }
}