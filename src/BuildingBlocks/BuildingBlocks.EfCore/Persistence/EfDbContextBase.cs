using BuildingBlocks.Shared.Domains;
using BuildingBlocks.Shared.Domains.Interfaces;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EfCore.Persistence.Commons;

public abstract class EfDbContextBase(DbContextOptions options, IInMemoryBus bus) : DbContext(options)
{
    private IDbContextTransaction? _transaction;


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTrackingEntities();
        var result =  await base.SaveChangesAsync(cancellationToken);
        
        await DispatchDomainEventsAsync();
        return result;
    }
    
    private void UpdateTrackingEntities()
    {
        // Lấy tất cả các entity đang được thêm (Added) hoặc sửa (Modified)
        var entries = ChangeTracker.Entries<IDateTracking>(); // Giả sử interface của bạn là IAuditable

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTimeOffset.UtcNow;
                entry.Entity.LastModifiedDate = DateTimeOffset.UtcNow; // Nếu bạn có dịch vụ lấy User
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedDate = DateTimeOffset.UtcNow;
            }
        }
    }

    private async Task DispatchDomainEventsAsync()
    {
        var aggregateRoots = ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(root => root.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await bus.Publish(domainEvent);
        }

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }
    }
    
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null) return;
        
        _transaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null) return;
        try
        {
            await SaveChangesAsync(cancellationToken);
            
            await _transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null) return;
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}