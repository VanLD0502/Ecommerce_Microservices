using BuildingBlocks.Shared.Commons;
using Ecommerce.Services.Carts.Api.Models.Dtos;

namespace Ecommerce.Services.Carts.Api.Models.Interfaces;

public interface ICartProductService
{
    Task<Result<ProductDto>> GetProductAsync(Guid productId);
}