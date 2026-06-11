using BuildingBlocks.Application.InMemoryBus;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using Ecommerce.Services.Catalog.Domain;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Catalog.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery : IQuery<IEnumerable<Product>>;

public class GetProductsQueryHandler(
    IEfUnitOfWork unitOfWork,
    ILogger<GetProductsQueryHandler> logger)
    : QueryHandler<GetProductsQuery, IEnumerable<Product>>
{
    private readonly IGenericEfRepository<Product, Guid> _productRepository = unitOfWork.Repository<Product, Guid>();

    protected override async Task<Result<IEnumerable<Product>>> HandleQueryAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetAllAsync(cancellationToken: cancellationToken);
            return Result<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách sản phẩm");
            return Result<IEnumerable<Product>>.Failure("Có lỗi xảy ra");
        }
    }

    
}
