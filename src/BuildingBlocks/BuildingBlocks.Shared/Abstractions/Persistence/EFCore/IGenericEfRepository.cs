using System.Linq.Expressions;
using BuildingBlocks.Shared.Entities.Interfaces;

namespace BuildingBlocks.Shared.Abstractions.Persistence.EFCore;

public interface IGenericEfRepository<T, in TKey>
    where T : class, IEntityBase<TKey>
{
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    void Delete(T entity);

    Task<T?> GetByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<List<TResult>> GetSelectAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool distinct = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<List<T>> GetPageAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<decimal> SumAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, decimal>> selector,
        CancellationToken cancellationToken = default);
}