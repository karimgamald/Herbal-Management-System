using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public interface IRepository<T> where T : class
{
    // ضفنا الـ CancellationToken وخليناه default
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, CancellationToken cancellationToken = default);
    Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, CancellationToken cancellationToken = default);
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    // دول بقوا void لأنهم بيعدلوا في الميموري بس
    void Update(T entity);
    void Remove(T entity);
}