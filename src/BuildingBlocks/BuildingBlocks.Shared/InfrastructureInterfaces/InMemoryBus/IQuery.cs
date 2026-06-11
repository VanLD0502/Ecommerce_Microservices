using BuildingBlocks.Shared.Commons;
using MediatR;

namespace BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;