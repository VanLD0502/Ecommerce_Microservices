using BuildingBlocks.Shared.Domains.Interfaces;

namespace BuildingBlocks.Shared.Domains;

public abstract class EntityTrackingBase<T> : IEntityBase<T>, IDateTracking
{
    public T Id { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}