namespace BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;

public interface IInMemoryBus : IQueryBus, ICommandBus, INotificationBus;