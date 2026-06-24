using BuildingBlocks.Auth;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.InfrastructureInterfaces.Caching;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Ecommerce.Services.Carts.Api.Features.Carts.Dtos;
using Ecommerce.Services.Carts.Api.Models.Constansts;
using Ecommerce.Services.Carts.Api.Models.Entities;
using Ecommerce.Services.Carts.Api.Models.Interfaces;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Carts.Api.Features.Carts.Queries.GetCart;

public class GetCartQueryHandler(
    ICacheService cacheService,
    IProductService productService,
    ILogger<GetCartQueryHandler> logger, IMapper mapper)
    : IQueryHandler<GetCartQuery, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        long customerId = request.CustomerId;
        try
        {
            var cart = await cacheService.GetAsync<Cart>(
                CartCacheKey.GetCartCacheKey(customerId), 
                cancellationToken);

            
            if (cart == null)
            {
                cart = new Cart(customerId);
                await cacheService.SetAsync(
                    CartCacheKey.GetCartCacheKey(customerId),
                    cart,
                    TimeSpan.FromDays(7),
                    cancellationToken);
            }

            if (cart.Items.Count == 0)
            {
                return Result<CartResponse>.Success(new CartResponse()
                {
                    CustomerId = customerId,
                    Items = []
                });
            }
            var variantIds = cart.Items.Select(i => i.ProductVariantId.ToString()).ToList();
            
            var listProductResult = await productService.GetProductVariantListAsync(variantIds);
            
            // Sử dụng {sanpham} thay vì @sanpham
            logger.LogInformation("Lấy danh sách sản phẩm trong giỏ hàng: {sanpham}", listProductResult.Value);

            var ListProductDto = listProductResult.Value;
            var productDist = ListProductDto.ToDictionary(p => p.VariantId);
            
            var cartResponse = new CartResponse()
            {
                CustomerId = customerId,
            };

            cartResponse.Items = mapper.Map<List<CartItemResponse>>(cart.Items);

            foreach (var item in  cartResponse.Items)
            {
                mapper.Map(productDist[item.ProductVariantId], item);
            }
            //A->B
            
            return Result<CartResponse>.Success(cartResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi lấy giỏ hàng của khách hàng {CustomerId}: {Message}", customerId, ex.Message);
            return Result<CartResponse>.Failure(ex.Message, EErrorCode.InternalServerError);
        }
    }
}
