using BuildingBlocks.Shared.Commons;

namespace BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;

public interface ICommandBus
{
    Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
}