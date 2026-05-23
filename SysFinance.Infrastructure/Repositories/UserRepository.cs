using Microsoft.EntityFrameworkCore;
using SysFinance.Domain.Entities;
using SysFinance.Domain.Interfaces;
using SysFinance.Infrastructure.Data;

namespace SysFinance.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
