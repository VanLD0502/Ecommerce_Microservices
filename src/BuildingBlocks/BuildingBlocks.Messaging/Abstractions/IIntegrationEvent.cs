using System.Windows.Input;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using MediatR;
using ICommand = BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus.ICommand;

namespace BuildingBlocks.Messaging.Abstractions;

public interface IIntegrationEvent : IEvent
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
