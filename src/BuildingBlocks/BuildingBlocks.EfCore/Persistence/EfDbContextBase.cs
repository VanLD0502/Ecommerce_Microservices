using BuildingBlocks.Shared.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EfCore.Persistence.Commons;

public abstract class EfDbContextBase(DbContextOptions options) : DbContext(options)
{
    private IDbContextTransaction? _transaction;


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTrackingEntities();
        
        return await base.SaveChangesAsync(cancellationToken);
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