using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Ecommerce.Services.Orders.Application.Features.Orders.Dtos;

namespace Ecommerce.Services.Orders.Application.Features.Commands.CreateOrder;

public record CreateOrderCommand(long CustomerId) : ICommand<CustomerOrderResponse>;
