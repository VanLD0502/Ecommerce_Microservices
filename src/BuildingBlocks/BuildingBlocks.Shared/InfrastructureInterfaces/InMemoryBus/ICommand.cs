using BuildingBlocks.Shared.Commons;
using MediatR;
namespace BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
public interface ICommand: IRequest<Result>;

public interface IEvent : IRequest<Unit>;