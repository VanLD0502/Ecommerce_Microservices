namespace BuildingBlocks.Shared.Entities.Interfaces;

public interface IEntityBase<T>
{
    T Id { get; set; }
}