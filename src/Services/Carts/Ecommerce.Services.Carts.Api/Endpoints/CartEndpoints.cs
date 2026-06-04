using Ecommerce.Services.Carts.Api.Models.Interfaces;
using Ecommerce.Services.Carts.Api.Models.Dtos;
using Ecommerce.Services.Carts.Api.Models.Entities;
using BuildingBlocks.Shared.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Services.Carts.Api.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder AddCartEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Thống nhất dùng {customerId} thay cho {userId} để đồng bộ toàn bộ API Group
        var group = endpoints.MapGroup("api/carts")
                             .WithTags("CartEndpoints")
                             .WithSummary("Đây là bộ API quản lý giỏ hàng")
                             .WithOpenApi();
        
        // GET /api/v1/carts/{customerId}
        group.MapGet("/{customerId}", GetCart)
            .WithName("GetCart")
            .WithSummary("Lấy giỏ hàng của user");
        
        // POST /api/v1/carts/{customerId}/items
        group.MapPost("/{customerId}/items", AddItem)
            .WithName("AddItem")
            .WithSummary("Thêm sản phẩm vào giỏ");
 
        // PUT /api/v1/carts/{customerId}/items/{productId}
        group.MapPut("/{customerId}/items/{productId}", UpdateQuantity)
            .WithName("UpdateQuantity")
            .WithSummary("Cập nhật số lượng sản phẩm");
        
        // DELETE /api/v1/carts/{customerId}/items/{productId}
        group.MapDelete("/{customerId}/items/{productId}", RemoveItem)
            .WithName("RemoveItem")
            .WithSummary("Xóa sản phẩm khỏi giỏ");
        
        // DELETE /api/v1/carts/{customerId}
        group.MapDelete("/{customerId}", ClearCart)
            .WithName("ClearCart")
            .WithSummary("Xóa toàn bộ giỏ hàng");

        return endpoints;
    }

    // 1. LẤY GIỎ HÀNG
    private static async Task<IResult> GetCart(int customerId, ICartService cartService)
    {
        var result = await cartService.GetCart(customerId);
        
        return result.IsSuccess 
            ? Results.Json(result.Value, statusCode: result.GetHttpStatusCode()) 
            : Results.Content(result.Message, statusCode:result.GetHttpStatusCode());
    }

    // 2. THÊM SẢN PHẨM VÀO GIỎ
    private static async Task<IResult> AddItem(int customerId, [FromBody] CartItemRequest cartItem, ICartService cartService)
    {
        var result = await cartService.AddItemToCart(customerId, cartItem.ProductId, cartItem.Quantity);


        return result.IsSuccess
            ? Results.Json(result.Value, statusCode: result.GetHttpStatusCode())
            : Results.Content(result.Message, statusCode: result.GetHttpStatusCode());
    }
    
    private static async Task<IResult> UpdateQuantity(int customerId, [FromBody] CartItemRequest cartItem, ICartService cartService)
    {
        var result = await cartService.UpdateQuantityAsync(customerId, cartItem.ProductId, cartItem.Quantity);
        
        return Results.Content(result.Message, statusCode: result.GetHttpStatusCode);
    }
    
    private static async Task<IResult> RemoveItem(int customerId, Guid productId, ICartService cartService)
    {
        var result = await cartService.RemoveItemFromCart(customerId, productId);
        
        return Results.Content(result.Message, statusCode: result.GetHttpStatusCode);
    }
    
    private static async Task<IResult> ClearCart(int customerId, ICartService cartService)
    {
        var result = await cartService.RemoveCart(customerId);
        
        return Results.Content(result.Message, statusCode: result.GetHttpStatusCode);
    }
}