namespace BuildingBlocks.Shared.InfrastructureInterfaces.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TMessage>(TMessage @event, CancellationToken cancellationToken = default)
        where TMessage: class, IIntegrationEvent;
}