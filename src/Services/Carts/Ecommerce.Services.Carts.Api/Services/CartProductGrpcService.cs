using BuildingBlocks.Grpc.Extensions;
using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using Ecommerce.Services.Carts.Api.Models.Dtos;
using Ecommerce.Services.Carts.Api.Models.Interfaces;
using Grpc.Core;
using MapsterMapper;

namespace Ecommerce.Services.Carts.Api.Services;

public class CartProductGrpcService(ILogger<CartProductGrpcService> logger, ProductGrpc.ProductGrpcClient grpcClient, IMapper mapper) : ICartProductService
{
    public async Task<Result<ProductDto>> GetProductAsync(Guid productId)
    {
        try
        {
            var productResponse = await grpcClient.GetProductByIdAsync(new GetProductRequest()
            {
                Id = productId.ToString()
            });


            var productDto = mapper.Map<ProductDto>(productResponse);

            return Result<ProductDto>.Success(productDto);
        }
        catch (RpcException ex)
        {
            logger.LogError(ex, ex.Message);
            return ex.ToResultFailure<ProductDto>();
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure(EErrorCode.InternalServerError, $"Có lỗi xảy ra khi lấy sản phầm với Id {productId}");
        }
    }
    
    
}