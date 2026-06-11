using BuildingBlocks.Shared.Domains.Interfaces;

namespace BuildingBlocks.Shared.Domains;

public abstract class EntityAuditBase<TKey> : EntityBase<TKey>, IAuditable
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}