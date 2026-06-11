using BuildingBlocks.Grpc.Extensions;
using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.Extensions;
using Ecommerce.Services.Carts.Api.Models.Dtos;
using Ecommerce.Services.Carts.Api.Models.Entities;
using Ecommerce.Services.Carts.Api.Models.Interfaces;
using Grpc.Core;
using MapsterMapper;

namespace Ecommerce.Services.Carts.Api.Services;

public class ProductService(ILogger<ProductService> logger, ProductGrpc.ProductGrpcClient grpcClient, IMapper mapper) : IProductService
{
    public async Task<Result<ProductDto>> GetProductAsync(Guid productId)
    {
        try
        {
            var productResponse = await grpcClient.GetProductByIdAsync(new GetProductByIdRequest()
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
            return Result<ProductDto>.Failure($"Có lỗi xảy ra khi lấy sản phầm với Id {productId}", EErrorCode.InternalServerError);
        }
    }

    public async Task<Result<CartValidationDto>> ValidateProducts(Cart cart)
    {
        try
        {
            var request = new ValidateProductsRequest();
            foreach (var cartItem in cart.Items)
            {
                // Tạo object riêng biệt
                var item = new ValidateProductItemDto
                {
                    Id = cartItem.ProductId.ToString(),
                    Price = cartItem.UnitPrice.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Quantity = cartItem.Quantity
                };

                // Add object đó vào
                request.Items.Add(item);
            }

            var response = await grpcClient.ValidateProductsAsync(request);
            
            var cartValidationDto = new  CartValidationDto();
            
            for (int i = 0; i < cart.Items.Count; i++)
            {
                var responseItem = response.Items[i];

                if (responseItem.ErrorStatus == ProductValidatedErrorStatus.None)
                {
                    continue;
                }
                
                cartValidationDto.errors.Add(new CartItemError
                {
                    productId = cart.Items[i].ProductId.ToString(),
                    errorMessage = GetErrorMessage(cart.Items[i], responseItem)
                });
            }

            
            cartValidationDto.isValid = cartValidationDto.errors.Count == 0;
            
            return Result<CartValidationDto>.Success(cartValidationDto,EErrorCode.SuccessCreated);
        }
        catch (RpcException ex)
        {
            logger.LogError(ex, ex.Message);
            return ex.ToResultFailure<CartValidationDto>();
        }
        catch (Exception e)
        {
            return Result<CartValidationDto>.Failure("Có lỗi", EErrorCode.InternalServerError);
        }
    }

    private string GetErrorMessage(CartItem cartItem, ProductValidatedItemDto  validatedItem)
    {
        if (validatedItem.ErrorStatus == ProductValidatedErrorStatus.PriceChanged)
        {
            return $"Sản phẩm {validatedItem.Name} có giá thay đổi từ {cartItem.UnitPrice} sang {validatedItem.Price}";
        }
        else if (validatedItem.ErrorStatus == ProductValidatedErrorStatus.OutOfStock)
        {
            return $"Số lượng mua sản phẩm {validatedItem.Name} lớn hơn lượng tồn kho";
        }
        else if (validatedItem.ErrorStatus == ProductValidatedErrorStatus.NotFound)
        {
            return $"Sản phẩm {cartItem.ProductName} hiện tại không tìm thấy trong hệ thống hoặc đã bị xóa";
        }

        return "";
    }
}