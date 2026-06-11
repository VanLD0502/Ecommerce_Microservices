using BuildingBlocks.Messaging.Abstractions;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Abstractions;

public class ConsumerBase<TEvent>(ISender sender, ILogger<ConsumerBase<TEvent>> logger) : IConsumer<TEvent> where TEvent: class, IIntegrationEvent 
{
    public virtual async Task Consume(ConsumeContext<TEvent> context)
    {
        logger.LogInformation("Consume message {@Message}", context.Message);
        await sender.Send(context.Message, context.CancellationToken);
    }
}