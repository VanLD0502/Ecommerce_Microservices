using BuildingBlocks.Shared.Entities.Interfaces;

namespace BuildingBlocks.Shared.Entities;

public abstract class EntityBase<T> : IEntityBase<T>
{
    public T Id { get; set; } = default!;
}