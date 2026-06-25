using BuildingBlocks.Shared.InfrastructureInterfaces.Messaging;
using MassTransit;

namespace BuildingBlocks.Messaging.Core;

public class EventPublisher(IPublishEndpoint publisher) : IEventPublisher
{
    public async Task PublishAsync<TMessage>(TMessage @event, CancellationToken cancellationToken = default) where TMessage : class, IIntegrationEvent
    {
        await publisher.Publish(@event, cancellationToken);
    }
}