using BuildingBlocks.Grpc.Extensions;
using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Shared.Extensions;
using Ecommerce.Services.Catalog.Application.Features.Products.Queries;
using Ecommerce.Services.Catalog.Application.Features.Products.Queries.GetVariantById;
using Grpc.Core;
using MediatR;

namespace Ecommerce.Services.Catalog.Api.GrpcServers;

public class ProductGrpcService(ISender sender, ILogger<ProductGrpcService> logger) : ProductGrpc.ProductGrpcBase
{
    public override async Task<GetVariantByIdResponse> GetVariantById(GetVariantByIdRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var variantId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid variant ID format"));
        } 
        logger.LogInformation($"Getting variant with variant ID {variantId}");
        var result = await sender.Send(new GetVariantByIdQuery(variantId));

        if (!result.IsSuccess)
        {
            throw result.ToRpcException();
        }
        
        var response = new GetVariantByIdResponse();
        var variant = result.Value;

        response.VariantId = variant.Id.ToString();
        response.AvailableStocks = variant.AvailableStocks;

        return response;
    }
    
    public override async Task<GetVariantsByIdsResponse> GetVariantsByIds(GetVariantsByIdsRequest request, ServerCallContext context)
    {
        var variantIds = request.VariantIds.Select(id => Guid.Parse(id)).ToList();
        logger.LogInformation($"Getting variants with variant IDs {string.Join(", ", variantIds)}");
        var result = await sender.Send(new GetVariantsByIdsQuery(variantIds));

        if (!result.IsSuccess)
        {
            throw result.ToRpcException();
        }
        
        var response = new GetVariantsByIdsResponse();

        var variants = result.Value;
        
        logger.LogInformation($"Getting variants {variants}", variants);
        
        foreach (var variantDto in variants)
        {
            var variant = new VariantGrpcDto();
            variant.UnitPrice = variantDto.Price.ToGrpcString();
            variant.ProductName = variantDto.ProductName;
            variant.ProductId = variantDto.ProductId.ToString();
            variant.AvailableStocks = variantDto.AvailableStocks;
            variant.VariantId = variantDto.Id.ToString();
            variant.VariantName = variantDto.VariantName;
            response.Variants.Add(variant);
        }
        return response;
    }
}