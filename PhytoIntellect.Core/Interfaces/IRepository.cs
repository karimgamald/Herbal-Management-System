using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public interface IRepository<T> where T : class
{
    // خلينا الـ includeProperties بعد الـ tracked عشان نحافظ على القديم
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null, CancellationToken cancellationToken = default);
    Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null, CancellationToken cancellationToken = default);
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}