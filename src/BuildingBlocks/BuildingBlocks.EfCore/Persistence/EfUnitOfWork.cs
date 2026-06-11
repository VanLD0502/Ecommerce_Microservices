
using System.Collections.Concurrent;
using BuildingBlocks.Shared.Domains.Interfaces;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using BuildingBlocks.Shared.Domains;

namespace BuildingBlocks.EfCore.Persistence.Commons;

public class EfUnitOfWork<TContext>(TContext context) : IEfUnitOfWork where TContext: EfDbContextBase
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    
    public IGenericEfRepository<T, TKey> Repository<T, TKey>() where T : class, IEntityBase<TKey>
    {
        var type = typeof(T);
        if (_repositories.TryGetValue(type, out var repo)) return (IGenericEfRepository<T, TKey>)repo;

        var newRepo = new GenericEfRepository<T, TKey, TContext>(context);
        _repositories[type] = newRepo!;
        return newRepo;
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await context.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await context.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await context.RollbackTransactionAsync(cancellationToken);
    }
    
    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await operation();
            await CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }
    
    
}