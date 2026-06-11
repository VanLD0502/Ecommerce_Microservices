using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using MediatR;

namespace BuildingBlocks.Application.InMemoryBus;

public class MediatRInMemoryBus(IMediator sender) : IInMemoryBus
{
    public Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        return sender.Send(query, cancellationToken);
    }

    public Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        return sender.Send(command, cancellationToken);
    }

    public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        return sender.Send(command, cancellationToken);
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return sender.Publish(notification, cancellationToken);
    }
}