using BuildingBlocks.Messaging.Abstractions;

namespace Ecommerce.Services.Orders.Contracts.Events;

public record OrderCreatedEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CustomerId { get; set; }
}
