using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Shared.InfrastructureInterfaces.Messaging;

namespace Ecommerce.Services.Orders.Contracts.Events;

public record OrderCreatedEvent : IIntegrationEvent

{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CustomerId { get; set; }
}
