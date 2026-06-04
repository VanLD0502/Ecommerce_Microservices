using System.Linq.Expressions;
using BuildingBlocks.Shared.Entities.Interfaces;
using BuildingBlocks.Shared.Abstractions.Persistence.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EfCore.Persistence.Commons;

public class GenericEfRepository<T, TKey, TDbContext> : IGenericEfRepository<T, TKey>
    where T : class, IEntityBase<TKey> where TDbContext : DbContext
{
    private readonly TDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericEfRepository(TDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public void Add(T entity) => _dbSet.Add(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public async Task<T?> GetByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(
            entity => entity.Id!.Equals(id),
            cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        query = query.Where(predicate);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> GetSelectAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool distinct = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        IQueryable<TResult> resultQuery = query.Select(selector);

        if (distinct)
        {
            resultQuery = resultQuery.Distinct();
        }

        return await resultQuery.ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetPageAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.CountAsync(predicate, cancellationToken);
    }

    public Task<decimal> SumAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, decimal>> selector,
        CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Where(predicate)
            .SumAsync(selector, cancellationToken);
    }
}