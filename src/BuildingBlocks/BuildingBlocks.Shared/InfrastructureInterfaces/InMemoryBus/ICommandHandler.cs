using BuildingBlocks.Shared.Commons;
using MediatR;

namespace BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
    
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;



public interface ICommandEventHandler<in TEvent> : IRequestHandler<TEvent, Unit> 
    where TEvent : IEvent;