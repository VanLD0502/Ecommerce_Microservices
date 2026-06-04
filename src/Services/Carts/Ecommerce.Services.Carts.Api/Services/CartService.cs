using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using Contracts.Abstractions.Caching;
using Ecommerce.Services.Carts.Api.Models.Constansts;
using Ecommerce.Services.Carts.Api.Models.Entities;
using Ecommerce.Services.Carts.Api.Models.Interfaces;

namespace Ecommerce.Services.Carts.Api.Services;

public class CartService(
    ICacheService cacheService,
    ILogger<CartService> logger, ICartProductService productService)
    : ICartService
{
    private static readonly TimeSpan CartExpiry =
        TimeSpan.FromDays(7);

    public async Task<Result<Cart?>> GetCart(int customerId)
    {
        try
        {
            var cart = await cacheService.GetAsync<Cart>(
                CartCacheKey.GetCartCacheKey(customerId));

            return Result<Cart?>.Success(cart);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return Result<Cart?>.Failure(
                EErrorCode.InternalServerError, 
                ex.Message);
        }
    }

    public async Task<Result<CartItem>> AddItemToCart(int customerId, Guid productId, int quantity)
    {
        try
        {
            var key = CartCacheKey.GetCartCacheKey(customerId);

            var cart = await cacheService.GetAsync<Cart>(key) ?? new Cart(customerId);
            

            var itemResponse =
                cart.Items.FirstOrDefault(x =>
                    x.ProductId == productId);

            if (itemResponse is not null)
            {
                itemResponse.Quantity +=  quantity;
            }
            else
            {
                var productResult = await productService.GetProductAsync(productId);

                if (!productResult.IsSuccess)
                {
                    return Result<CartItem>.Failure(productResult.ErrorCode, productResult.Message ?? "Có lỗi xảy ra");
                }
                
                var product = productResult.Value!;

                itemResponse = new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = quantity
                };
                cart.Items.Add(itemResponse);
            }

            await cacheService.SetAsync(
                key,
                cart,
                CartExpiry);

            return Result<CartItem>.Success(itemResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return Result<CartItem>.Failure(
                EErrorCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Result> RemoveItemFromCart(
        int customerId,
        Guid productId)
    {
        try
        {
            var key = CartCacheKey.GetCartCacheKey(customerId);

            var cart = await cacheService.GetAsync<Cart>(key);

            if (cart is null)
                return Result.Failure("Không tìm thấy giỏi hàng", EErrorCode.NotFound);

            cart.Items.RemoveAll(x => x.ProductId == productId);

            await cacheService.SetAsync(key, cart, CartExpiry);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return Result.Failure(
                ex.Message,
                EErrorCode.InternalServerError);
        }
    }

    public async Task<Result> UpdateQuantityAsync(
        int customerId, Guid productId, int quantity)
    {
        try
        {
            var key = CartCacheKey.GetCartCacheKey(customerId);

            var cart = await cacheService.GetAsync<Cart>(key);

            if (cart is null)
            {
                return Result.Failure("Cart not found", EErrorCode.NotFound);
            }

            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem is null)
            {
                return Result.Failure("Item not found", EErrorCode.NotFound);
            }

            existingItem.Quantity = quantity;

            if (existingItem.Quantity <= 0)
            {
                cart.Items.Remove(existingItem);
            }

            await cacheService.SetAsync(key, cart, CartExpiry);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return Result.Failure( ex.Message, EErrorCode.InternalServerError);
        }
    }

    public async Task<Result> RemoveCart(int customerId)
    {
        try
        {
            await cacheService.RemoveAsync(CartCacheKey.GetCartCacheKey(customerId));
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            return Result.Failure( ex.Message, EErrorCode.InternalServerError);
        }
    }
}