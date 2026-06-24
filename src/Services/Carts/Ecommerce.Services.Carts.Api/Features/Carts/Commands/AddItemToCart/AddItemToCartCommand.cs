using BuildingBlocks.Auth;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.InfrastructureInterfaces.Caching;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Ecommerce.Services.Carts.Api.Models.Constansts;
using Ecommerce.Services.Carts.Api.Models.Entities;
using Ecommerce.Services.Carts.Api.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Carts.Api.Features.Carts.Commands.AddItemToCart;

public record AddItemToCartCommand(long CustomerId, Guid ProductVariantId, int Quantity) : ICommand<CartItem>;

public class AddItemToCartCommandHandler(
    ICacheService cacheService,
    IProductService productService,
    ILogger<AddItemToCartCommandHandler> logger)
    : ICommandHandler<AddItemToCartCommand, CartItem>
{
    private static readonly TimeSpan CartExpiry = TimeSpan.FromDays(7);

    public async Task<Result<CartItem>> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        long customerId = request.CustomerId;
        try
        {
            var key = CartCacheKey.GetCartCacheKey(customerId);
            var cart = await cacheService.GetAsync<Cart>(key, cancellationToken) ?? new Cart(customerId);

            var itemResponse = cart.Items.FirstOrDefault(x => x.ProductVariantId == request.ProductVariantId);

            if (itemResponse is not null)
            {
                itemResponse.Quantity += request.Quantity;
            }
            else
            {
                logger.LogInformation("Đang thêm sản phẩm {ProductVariantId} vào giỏ hàng của khách hàng {CustomerId}", request.ProductVariantId, customerId);
                var productResult = await productService.GetProductVariantAsync(request.ProductVariantId);

                if (!productResult.IsSuccess)
                {
                    return Result<CartItem>.Failure(productResult.Message ?? "Có lỗi xảy ra", productResult.ErrorCode);
                }
                

                var product = productResult.Value!;

                itemResponse = new CartItem
                {
                    ProductVariantId = request.ProductVariantId,
                    Quantity = request.Quantity
                };
                cart.Items.Add(itemResponse);
            }
            

            await cacheService.SetAsync(key, cart, CartExpiry, cancellationToken);

            return Result<CartItem>.Success(itemResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi thêm sản phẩm vào giỏ hàng của khách hàng {CustomerId}: {Message}", customerId, ex.Message);
            return Result<CartItem>.Failure(ex.Message, EErrorCode.InternalServerError);
        }
    }
}
