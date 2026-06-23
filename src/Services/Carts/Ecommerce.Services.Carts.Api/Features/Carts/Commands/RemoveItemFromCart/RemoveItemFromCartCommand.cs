using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.InfrastructureInterfaces.Caching;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Ecommerce.Services.Carts.Api.Models.Constansts;
using Ecommerce.Services.Carts.Api.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Carts.Api.Features.Carts.Commands.RemoveItemFromCart;

public record RemoveItemFromCartCommand(int CustomerId, Guid ProductVariantId) : ICommand;

public class RemoveItemFromCartCommandHandler(
    ICacheService cacheService,
    ILogger<RemoveItemFromCartCommandHandler> logger)
    : ICommandHandler<RemoveItemFromCartCommand>
{
    private static readonly TimeSpan CartExpiry = TimeSpan.FromDays(7);

    public async Task<Result> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var key = CartCacheKey.GetCartCacheKey(request.CustomerId);
            var cart = await cacheService.GetAsync<Cart>(key, cancellationToken);

            if (cart is null)
            {
                return Result.Failure("Không tìm thấy giỏ hàng", EErrorCode.NotFound);
            }

            cart.Items.RemoveAll(x => x.ProductVariantId == request.ProductVariantId);

            await cacheService.SetAsync(key, cart, CartExpiry, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi xóa sản phẩm khỏi giỏ hàng của khách hàng {CustomerId}: {Message}", request.CustomerId, ex.Message);
            return Result.Failure(ex.Message, EErrorCode.InternalServerError);
        }
    }
}
