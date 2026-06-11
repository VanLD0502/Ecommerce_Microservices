using BuildingBlocks.Application.InMemoryBus;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using Ecommerce.Services.Catalog.Domain;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Catalog.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<Product>;

public class GetProductByIdQueryHandler(
    IEfUnitOfWork unitOfWork,
    ILogger<GetProductByIdQueryHandler> logger)
    : QueryHandler<GetProductByIdQuery, Product>
{
    private readonly IGenericEfRepository<Product, Guid> _productRepository = unitOfWork.Repository<Product, Guid>();

    protected override async Task<Result<Product>> HandleQueryAsync(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(query.Id, cancellationToken);

            if (product == null)
            {
                return Result<Product>.Failure("Product Not Found", EErrorCode.NotFound);
            }
            return Result<Product>.Success(product);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Có lỗi xảy ra khi lấy sản phẩm {query.Id}";
            logger.LogError(ex, errorMessage);
            return Result<Product>.Failure(errorMessage, EErrorCode.InternalServerError);
        }
    }

}
