using BuildingBlocks.Shared.Domains.Interfaces;

namespace BuildingBlocks.Shared.Domains;

public abstract class EntityBase<T> : IEntityBase<T>
{
    public T Id { get; set; } = default!;
}