using SysFinance.Domain.Entities;

namespace SysFinance.Domain.Interfaces;

public interface IAssetHistoryRepository : IRepository<AssetHistory>
{
    Task<List<AssetHistory>> GetAllByUserId(Guid userId);
}
