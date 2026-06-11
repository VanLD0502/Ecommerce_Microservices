using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using MediatR;

namespace BuildingBlocks.Application.InMemoryBus;


public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    protected abstract Task<Result<TResponse>> HandleQueryAsync(TQuery query, CancellationToken cancellationToken);
    public Task<Result<TResponse>> Handle(TQuery request, CancellationToken cancellationToken) => HandleQueryAsync(request, cancellationToken);
}