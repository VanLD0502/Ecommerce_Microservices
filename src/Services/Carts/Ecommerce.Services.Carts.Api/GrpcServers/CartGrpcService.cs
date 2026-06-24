using BuildingBlocks.Auth;
using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Shared.Extensions;
using Ecommerce.Services.Carts.Api.Features.Carts.Queries.GetCart;
using Grpc.Core;
using MapsterMapper;
using MediatR;

namespace Ecommerce.Services.Carts.Api.GrpcServers;

public class CartGrpcService(ISender sender, IMapper mapper) : CartGrpc.CartGrpcBase
{
    public override async Task<GetCartByIdResponse> GetCartByCustomerId(GetCartByIdRequest request, ServerCallContext context)
    {
        var result = await sender.Send(new GetCartQuery(request.CustomerId));
        if (!result.IsSuccess)
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Message ?? "Error fetching cart"));
        }
        
        var cartResponse = result.Value;
        var response = new GetCartByIdResponse
        {
            CustomerId = cartResponse.CustomerId
        };
        // Duyệt qua từng item và add thủ công vào RepeatedField gRPC
        foreach (var item in cartResponse.Items)
        {
            response.Items.Add(new CartItemDto
            {
                VariantId = item.ProductVariantId.ToString(),
                ProductName = item.ProductName ?? string.Empty,
                VariantName = item.VariantName ?? string.Empty,
                UnitPrice = item.UnitPrice.ToGrpcString(),
                AvailableStocks = item.AvailableStocks,
                Quantity = item.Quantity
            });
        }
        return response;
    }
}
