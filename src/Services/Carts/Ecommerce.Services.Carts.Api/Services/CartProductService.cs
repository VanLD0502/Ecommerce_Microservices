using BuildingBlocks.Shared.Commons;
using Ecommerce.Services.Carts.Api.Models.Dtos;
using Ecommerce.Services.Carts.Api.Models.Interfaces;

namespace Ecommerce.Services.Carts.Api.Services;

public class CartProductService(HttpClient httpClient, ILogger<CartProductService> logger) : ICartProductService
{
    
    public async Task<Result<ProductDto>> GetProductAsync(Guid productId)
    {
        try
        {   
            var response = await httpClient.GetAsync($"api/products/{productId}");

            if (response.IsSuccessStatusCode)
            {
                var productDto = response.Content.ReadFromJsonAsync<ProductDto>().Result;
                return Result<ProductDto>.Success(productDto);
            }
            
            return Result<ProductDto>.Failure(response.ReasonPhrase);
        }
        catch (Exception e)
        {
            logger .LogError(e, e.Message);
            return Result<ProductDto>.Failure(e.Message);
        }
    }
}