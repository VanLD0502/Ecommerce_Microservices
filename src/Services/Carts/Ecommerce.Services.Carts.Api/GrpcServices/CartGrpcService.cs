using BuildingBlocks.Grpc.Services;
using Ecommerce.Services.Carts.Api.Features.Carts.Queries.GetCart;
using Grpc.Core;
using MediatR;

namespace Ecommerce.Services.Carts.Api.GrpcServices;

public class CartGrpcService(ISender sender) : CartGrpc.CartGrpcBase
{
    public override async Task<GetCartByIdResponse> GetCartByCustomerId(GetCartByIdRequest request, ServerCallContext context)
    {
        var result = await sender.Send(new GetCartQuery(request.CustomerId));

        if (!result.IsSuccess)
        {
            throw new RpcException(new Status(StatusCode.Internal, result.Message ?? "Error fetching cart"));
        }

        var cart = result.Value;
        var response = new GetCartByIdResponse
        {
            CustomerId = request.CustomerId
        };

        if (cart != null)
        {
            foreach (var item in cart.Items)
            {
                response.Items.Add(new CartItemDto
                {
                    ProductId = item.ProductId.ToString(),
                    UnitPrice = item.UnitPrice.ToString("G29", System.Globalization.CultureInfo.InvariantCulture),
                    Quantity = item.Quantity,
                    ProductName = item.ProductName
                });
            }
            response.TotalPrice = cart.TotalPrice.ToString("G29", System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            response.TotalPrice = "0";
        }

        return response;
    }
}
