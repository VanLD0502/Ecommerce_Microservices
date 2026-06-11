using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using MediatR;

namespace BuildingBlocks.Application.InMemoryBus;

public abstract class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{

    protected abstract Task<Result<TResponse>> HandleCommandAsync(TCommand command, CancellationToken cancellationToken);
    
    public async Task<Result<TResponse>> Handle(TCommand request, CancellationToken cancellationToken =  default)
    { 
        return await HandleCommandAsync(request, cancellationToken);
    }
}

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    protected abstract Task<Result> HandleCommandAsync(TCommand command, CancellationToken cancellationToken);
    
    public async Task<Result> Handle(TCommand request, CancellationToken cancellationToken =  default)
    {
        return (await HandleCommandAsync(request, cancellationToken));
    }
}

public abstract class CommandEventHandler<TEvent> : ICommandEventHandler<TEvent> where TEvent : IEvent
{
    protected abstract Task HandleCommandEventAsync(TEvent @event, CancellationToken cancellationToken);
    
    public async Task<Unit> Handle(TEvent request, CancellationToken cancellationToken =  default)
    {
        await HandleCommandEventAsync(request, cancellationToken);
        return Unit.Value;
    }
}