using BuildingBlocks.Shared.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BuildingBlocks.Shared.Abstractions.Persistence.EFCore;

public interface IEfUnitOfWork<TContext> where TContext: DbContext
{
    IGenericEfRepository<T, TKey> Repository<T, TKey>() where T : class, IEntityBase<TKey>;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);

    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}