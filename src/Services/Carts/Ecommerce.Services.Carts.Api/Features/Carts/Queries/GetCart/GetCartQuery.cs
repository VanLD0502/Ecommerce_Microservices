using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.InfrastructureInterfaces.Caching;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Ecommerce.Services.Carts.Api.Models.Constansts;
using Ecommerce.Services.Carts.Api.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Carts.Api.Features.Carts.Queries.GetCart;

public record GetCartQuery(int CustomerId) : IQuery<Cart?>;

public class GetCartQueryHandler(
    ICacheService cacheService,
    ILogger<GetCartQueryHandler> logger)
    : IQueryHandler<GetCartQuery, Cart?>
{
    public async Task<Result<Cart?>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await cacheService.GetAsync<Cart>(
                CartCacheKey.GetCartCacheKey(request.CustomerId),
                cancellationToken);

            return Result<Cart?>.Success(cart);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi lấy giỏ hàng của khách hàng {CustomerId}: {Message}", request.CustomerId, ex.Message);
            return Result<Cart?>.Failure(ex.Message, EErrorCode.InternalServerError);
        }
    }
}
