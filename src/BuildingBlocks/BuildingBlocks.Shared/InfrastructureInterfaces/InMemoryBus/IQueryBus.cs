using BuildingBlocks.Shared.Commons;

namespace BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;

public interface IQueryBus
{
    Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}