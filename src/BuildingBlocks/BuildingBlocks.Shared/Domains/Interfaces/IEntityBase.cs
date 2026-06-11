namespace BuildingBlocks.Shared.Domains.Interfaces;

public interface IEntityBase<T>
{
    T Id { get; set; }
}