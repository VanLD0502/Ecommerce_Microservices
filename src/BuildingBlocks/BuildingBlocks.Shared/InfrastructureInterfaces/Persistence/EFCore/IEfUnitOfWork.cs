using BuildingBlocks.Shared.Domains.Interfaces;
namespace BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;

public interface IEfUnitOfWork
{
    IGenericEfRepository<T, TKey> Repository<T, TKey>() where T : class, IEntityBase<TKey>;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);

    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}