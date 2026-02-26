using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Core.Interfaces.RepositoryInterfaces;
using PhytoIntellect.Infrastructure.Presistence;
using System.Linq.Expressions;

namespace PhytoIntellect.Infrastructure.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
    {
        IQueryable<T> query = _dbSet;

        if (!tracked)
            query = query.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        return await query.ToListAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
    {
        IQueryable<T> query = _dbSet;

        if (!tracked)
            query = query.AsNoTracking();

        return filter == null
            ? await query.FirstOrDefaultAsync()
            : await query.FirstOrDefaultAsync(filter);
    }

    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }
}