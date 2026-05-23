using SysFinance.Domain.Entities;

namespace SysFinance.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
