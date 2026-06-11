using BuildingBlocks.Shared.Commons;
using MediatR;

namespace BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
    