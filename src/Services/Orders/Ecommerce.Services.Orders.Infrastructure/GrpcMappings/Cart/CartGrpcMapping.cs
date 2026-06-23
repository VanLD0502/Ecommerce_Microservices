using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Shared.Extensions;
using Ecommerce.Services.Orders.Application.Commons.Dtos.Cart;
using Mapster;
using CartGrpcItemDto = BuildingBlocks.Grpc.Services.CartItemDto;
using CartItemDto = Ecommerce.Services.Orders.Application.Commons.Dtos.Cart.CartItemDto;

namespace Ecommerce.Services.Orders.Infrastructure.Mappings.Cart;

public class CartGrpcMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CartGrpcItemDto, CartItemDto>()
            .Map(dest => dest.UnitPrice, src => src.UnitPrice.FromGrpcString());

    }
}