namespace BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;

public interface INotificationBus
{
    Task Publish(object notification, CancellationToken cancellationToken = default);
}