using BuildingBlocks.Shared.Commons;
using Ecommerce.Services.Catalog.Api.Models.Dtos;
using Ecommerce.Services.Catalog.Api.Models.Entities;

namespace Ecommerce.Services.Catalog.Api.Models.Interfaces;

public interface IProductService
{
    Task<Result<IEnumerable<Product>>> GetProductsAsync();
    Task<Result<Product>> GetProductByIdAsync(Guid id);
    Task<Result<Product>> AddProductAsync(ProductRequest productRequest);
    Task<Result<Product>> UpdateProductAsync(Guid Id,  ProductRequest productRequest);
    Task<Result<Product>> DeleteProductAsync(Guid id);
}