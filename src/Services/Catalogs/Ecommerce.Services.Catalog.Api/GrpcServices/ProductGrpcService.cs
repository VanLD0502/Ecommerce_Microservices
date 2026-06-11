using BuildingBlocks.Grpc.Extensions;
using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.Extensions;
using Ecommerce.Services.Catalog.Application.Features.Products.Queries.GetProductById;
using Grpc.Core;
using MediatR;

namespace Ecommerce.Services.Catalog.Api.GrpcServices;

public class ProductGrpcService(ISender sender) : ProductGrpc.ProductGrpcBase
{
    public override async Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out Guid productId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid product id"));
        }

        var productResult = await sender.Send(new GetProductByIdQuery(productId));

        if (!productResult.IsSuccess)
        {
            throw productResult.ToRpcException();
        }

        var product = productResult.Value;
        var response = new GetProductByIdResponse
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Price = product.Price.ToGrpcString()
        };

        return response;
    }

    public override async Task<ValidateProductsResponse> ValidateProducts(ValidateProductsRequest request,
        ServerCallContext context)
    {
        var response = new ValidateProductsResponse();
        foreach (ValidateProductItemDto item in request.Items)
        {
            if (!Guid.TryParse(item.Id, out Guid productId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid product id"));
            }

            var productResult = await sender.Send(new GetProductByIdQuery(productId));
            
            if (productResult.ErrorCode == EErrorCode.NotFound)
            {
                response.Items.Add(new ProductValidatedItemDto()
                {
                    Id = item.Id,
                    Name = "",
                    Price = "0",
                    Quantity = 0,
                    ErrorStatus = ProductValidatedErrorStatus.NotFound
                });
                continue;
            }

            if (!productResult.IsSuccess)
            {
                throw productResult.ToRpcException();
            }

            var product = productResult.Value;

            var ProductValidatedItemDto = new ProductValidatedItemDto()
            {
                Id = item.Id,
                Name = product.Name,
                Quantity = item.Quantity,
                Price = product.Price.ToGrpcString(),
                ErrorStatus = ProductValidatedErrorStatus.None
            };

            if (item.Quantity > product.Stocks)
            {
                ProductValidatedItemDto.Quantity = product.Stocks;
                ProductValidatedItemDto.ErrorStatus = ProductValidatedErrorStatus.OutOfStock;
            }

            if (!decimal.TryParse(item.Price, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal clientPrice) || clientPrice != product.Price)
            {
                ProductValidatedItemDto.ErrorStatus = ProductValidatedErrorStatus.PriceChanged;
            }
            
            response.Items.Add(ProductValidatedItemDto);
        }

        return response;
    }
}