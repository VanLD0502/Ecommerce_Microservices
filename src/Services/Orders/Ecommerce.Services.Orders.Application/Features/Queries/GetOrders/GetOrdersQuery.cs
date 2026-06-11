using BuildingBlocks.Application.InMemoryBus;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using Ecommerce.Services.Orders.Application.Commons.Dtos.Orders;
using Ecommerce.Services.Orders.Domain;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Orders.Application.Features.Queries.GetCustomerOrders;

public record GetCustomerOrdersQuery(long CustomerId) : IQuery<List<CustomerOrderResponse>>;

public class GetCustomerOrdersQueryHandler(
    IEfUnitOfWork unitOfWork,
    ILogger<GetCustomerOrdersQueryHandler> logger, IMapper mapper)
    : QueryHandler<GetCustomerOrdersQuery, List<CustomerOrderResponse>>
{
    protected override async Task<Result<List<CustomerOrderResponse>>> HandleQueryAsync(GetCustomerOrdersQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting orders for customer: {CustomerId}", query.CustomerId);

        var orderRepo = unitOfWork.Repository<Order, Guid>();

        // Fetch orders using repository method, without calling AsQueryable()
        var orders = await orderRepo.GetAllAsync(
            predicate: o => o.CustomerId == query.CustomerId,
            orderBy: q => q.OrderByDescending(o => o.CreatedDate),
            cancellationToken: cancellationToken,
            includes: o => o.OrderItems
        );
        
        var response = mapper.Map<List<CustomerOrderResponse>>(orders);

        return Result<List<CustomerOrderResponse>>.Success(response);
    }
    
}
