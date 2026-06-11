using BuildingBlocks.Application.InMemoryBus;
using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using Ecommerce.Services.Catalog.Application.Commons.Dtos.Products;
using Ecommerce.Services.Catalog.Domain;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Catalog.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stocks) : ICommand<ProductResponse>;

public class CreateProductCommandHandler(
    IEfUnitOfWork unitOfWork,
    ILogger<CreateProductCommandHandler> logger, IMapper mapper)
    : CommandHandler<CreateProductCommand, ProductResponse>
{
    private readonly IGenericEfRepository<Product, Guid> _productRepository = unitOfWork.Repository<Product, Guid>();

    protected override async Task<Result<ProductResponse>> HandleCommandAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var product = new Product(
                command.Name,
                command.Description,
                command.Price,
                command.Stocks
            );

            _productRepository.Add(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            var response = mapper.Map<ProductResponse>(product);
            
            return Result<ProductResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Có lỗi xảy ra khi thêm sản phẩm: {Name}", command.Name);
            return Result<ProductResponse>.Failure("Có lỗi xảy ra khi thêm sản phẩm");
        }
    }
    
}
