using Ecommerce.Services.Carts.Api.Models.Dtos;
using Ecommerce.Services.Carts.Api.Features.Carts.Queries.GetCart;
using Ecommerce.Services.Carts.Api.Features.Carts.Commands.AddItemToCart;
using Ecommerce.Services.Carts.Api.Features.Carts.Commands.RemoveItemFromCart;
using Ecommerce.Services.Carts.Api.Features.Carts.Commands.UpdateQuantity;
using Ecommerce.Services.Carts.Api.Features.Carts.Commands.RemoveCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Services.Carts.Api.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder AddCartEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/carts")
                             .WithTags("CartEndpoints")
                             .WithSummary("Đây là bộ API quản lý giỏ hàng")
                             .WithOpenApi();
        
        // GET /api/carts/{customerId}
        group.MapGet("/{customerId}", GetCart)
            .WithName("GetCart")
            .WithSummary("Lấy giỏ hàng của user");
        
        // POST /api/carts/{customerId}/items
        group.MapPost("/{customerId}/items", AddItem)
            .WithName("AddItem")
            .WithSummary("Thêm sản phẩm vào giỏ");
 
        // PUT /api/carts/{customerId}/items/{productId}
        group.MapPut("/{customerId}/items/{productId}", UpdateQuantity)
            .WithName("UpdateQuantity")
            .WithSummary("Cập nhật số lượng sản phẩm");
        
        // DELETE /api/carts/{customerId}/items/{productId}
        group.MapDelete("/{customerId}/items/{productId}", RemoveItem)
            .WithName("RemoveItem")
            .WithSummary("Xóa sản phẩm khỏi giỏ");
        
        // DELETE /api/carts/{customerId}
        group.MapDelete("/{customerId}", ClearCart)
            .WithName("ClearCart")
            .WithSummary("Xóa toàn bộ giỏ hàng");

        return endpoints;
    }

    // 1. LẤY GIỎ HÀNG
    private static async Task<IResult> GetCart(int customerId, ISender sender)
    {
        var result = await sender.Send(new GetCartQuery(customerId));
        
        return result.IsSuccess 
            ? Results.Json(result.Value, statusCode: result.GetHttpStatusCode()) 
            : Results.Content(result.Message, statusCode: result.GetHttpStatusCode());
    }

    // 2. THÊM SẢN PHẨM VÀO GIỎ
    private static async Task<IResult> AddItem(int customerId, [FromBody] CartItemRequest cartItem, ISender sender)
    {
        var result = await sender.Send(new AddItemToCartCommand(customerId, cartItem.VariantId, cartItem.Quantity));

        return result.IsSuccess
            ? Results.Json(result.Value, statusCode: result.GetHttpStatusCode())
            : Results.Content(result.Message, statusCode: result.GetHttpStatusCode());
    }
    
    // 3. CẬP NHẬT SỐ LƯỢNG SẢN PHẨM
    private static async Task<IResult> UpdateQuantity(int customerId, [FromBody] CartItemRequest cartItem, ISender sender)
    {
        var result = await sender.Send(new UpdateQuantityCommand(customerId, cartItem.VariantId, cartItem.Quantity));
        
        return Results.Content(result.Message, statusCode: result.GetHttpStatusCode);
    }
    
    // 4. XÓA SẢN PHẨM KHỎI GIỎ
    private static async Task<IResult> RemoveItem(int customerId, Guid productId, ISender sender)
    {
        var result = await sender.Send(new RemoveItemFromCartCommand(customerId, productId));
        
        return Results.Content(result.Message, statusCode: result.GetHttpStatusCode);
    }
    
    // 5. XÓA TOÀN BỘ GIỎ HÀNG
    private static async Task<IResult> ClearCart(int customerId, ISender sender)
    {
        var result = await sender.Send(new RemoveCartCommand(customerId));
        
        return Results.Content(result.Message, statusCode: result.GetHttpStatusCode);
    }
}