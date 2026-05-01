using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null, CancellationToken cancellationToken = default);
    Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true,string? includeProperties = null, CancellationToken cancellationToken = default);
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<User> GetQueryable(bool tracked = false);
}