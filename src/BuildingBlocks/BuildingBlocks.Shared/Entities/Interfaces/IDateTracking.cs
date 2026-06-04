namespace BuildingBlocks.Shared.Entities.Interfaces;

public interface IDateTracking
{
    DateTimeOffset CreatedDate { get; set; }
    DateTimeOffset? LastModifiedDate { get; set; }
}