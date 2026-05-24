using Microsoft.EntityFrameworkCore;
using SysFinance.Domain.Entities;
using SysFinance.Domain.Interfaces;
using SysFinance.Infrastructure.Data;

namespace SysFinance.Infrastructure.Repositories;

public class AssetHistoryRepository : Repository<AssetHistory>, IAssetHistoryRepository
{
    public AssetHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<List<AssetHistory>> GetAllByUserId(Guid userId)
    {
        var histories = await _context.AssetHistories
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Date)
            .ToListAsync();
        return histories;
    }
}
