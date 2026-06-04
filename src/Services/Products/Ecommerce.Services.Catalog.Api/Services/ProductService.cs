using BuildingBlocks.Shared.Abstractions.Persistence.EFCore;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using Ecommerce.Services.Catalog.Api.Infrastructure;
using Ecommerce.Services.Catalog.Api.Models.Dtos;
using Ecommerce.Services.Catalog.Api.Models.Entities;
using Ecommerce.Services.Catalog.Api.Models.Interfaces;
using MapsterMapper;

namespace Ecommerce.Services.Catalog.Api.Services;

public class ProductService(IEfUnitOfWork<ProductDbContext> _unitOfWork, IMapper _mapper,
    ILogger<ProductService> _logger) : IProductService
{
    private IGenericEfRepository<Product, Guid> _productEfRepository => _unitOfWork.Repository<Product, Guid>();

    public async Task<Result<IEnumerable<Product>>> GetProductsAsync()
    {
        try
        {
            var products = await _productEfRepository.GetAllAsync();
            return Result<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xày ra khi lấy danh sách sản phẩm");
            return Result<IEnumerable<Product>>.Failure("Có lỗi xảy ra");
        }
    }
    
    public async Task<Result<Product>> GetProductByIdAsync(Guid Id)
    {
        try
        {
            var product = await _productEfRepository.GetByIdAsync(Id);

            if (product == null)
            {
                return  Result<Product>.Failure(EErrorCode.NotFound, "Product Not Found");
            }
            return Result<Product>.Success(product);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Có lỗi xảy ra khi lấy sản phẩm {Id}";
            _logger.LogError(ex, errorMessage);
            return Result<Product>.Failure(EErrorCode.InternalServerError, errorMessage);
        }
    }

    public async Task<Result<Product>> AddProductAsync(ProductRequest productRequest)
    {
        try
        {
            var product = _mapper.Map<Product>(productRequest);
            _productEfRepository.Add(product);
            
            await _unitOfWork.SaveChangesAsync();
            
            return Result<Product>.Success(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy khi thêm sản phầm: {request}", productRequest );
            return Result<Product>.Failure("Có lỗi xảy ra khi thêm sản phầm");
        }
    }

    public async Task<Result<Product>> UpdateProductAsync(Guid Id,  ProductRequest productRequest)
    {
        try
        {
            var existsProduct = await _productEfRepository.GetByIdAsync(Id);
            
            if (existsProduct == null)
            {
                return Result<Product>.Failure(EErrorCode.NotFound, "Product Not Found");
            }
            
            _mapper.Map(productRequest, existsProduct);
            _productEfRepository.Update(existsProduct);
            await _unitOfWork.SaveChangesAsync();
            
            return Result<Product>.Success(existsProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi thêm sản phầm {id}", Id);
            return Result<Product>.Failure($"Có lỗi xảy ra khi thêm sản phầm {Id}");
        }
    }

    public async Task<Result<Product>> DeleteProductAsync(Guid id)
    {
        try
        {
            var  existsProduct = await _productEfRepository.GetByIdAsync(id);

            if (existsProduct == null)
            {
                return Result<Product>.Failure(EErrorCode.NotFound, "Product Not Found");
            }
            
            _productEfRepository.Delete(existsProduct);
            
            await _unitOfWork.SaveChangesAsync();
            
            return Result<Product>.Success(existsProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Có lỗi xảy ra khi thêm sản phầm {id}", id);
            return Result<Product>.Failure("Co loi");
        }
    }
}