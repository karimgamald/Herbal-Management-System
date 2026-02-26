using System;
using System.Linq.Expressions;

public interface ICRUDRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null!, bool tracked = true);
    Task<T?> GetAsync(Expression<Func<T, bool>> filter = null!, bool tracked = true);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
}
