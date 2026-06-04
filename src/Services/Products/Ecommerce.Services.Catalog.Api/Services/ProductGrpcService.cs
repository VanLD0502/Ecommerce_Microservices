

using BuildingBlocks.Grpc.Extensions;
using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Shared.Abstractions.Persistence.EFCore;
using Ecommerce.Services.Catalog.Api.Infrastructure;
using Ecommerce.Services.Catalog.Api.Models.Dtos;
using Ecommerce.Services.Catalog.Api.Models.Entities;
using Ecommerce.Services.Catalog.Api.Models.Interfaces;
using Grpc.Core;
using MapsterMapper;

namespace Ecommerce.Services.Catalog.Api.Services;

public class ProductGrpcService(IProductService productService, IMapper mapper) : ProductGrpc.ProductGrpcBase
{
    public override async Task<ProductResponse> GetProductById(GetProductRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out Guid productId))
        {
            //400 Bad request
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid product id"));
        }
        
        var productResult = await productService.GetProductByIdAsync(productId);

        if (!productResult.IsSuccess)
        {
            throw productResult.ToRpcException();
        }
        
        var response = mapper.Map<ProductResponse>(productResult.Value);

        return response;
    }
}