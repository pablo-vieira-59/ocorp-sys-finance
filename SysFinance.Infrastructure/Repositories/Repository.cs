using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SysFinance.Domain.Interfaces;
using SysFinance.Infrastructure.Data;

namespace SysFinance.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    public readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public AppDbContext DbContext { get { return _context; } }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(
    Expression<Func<T, bool>> predicate,
    params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        query = includes.Aggregate(query,
            (current, include) => current.Include(include));

        return await query.Where(predicate).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
