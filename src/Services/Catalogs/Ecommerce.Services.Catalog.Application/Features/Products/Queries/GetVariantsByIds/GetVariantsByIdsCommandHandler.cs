using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using Ecommerce.Services.Catalog.Application.Commons.Dtos.Products;
using Ecommerce.Services.Catalog.Application.Features.Products.Dtos;
using Ecommerce.Services.Catalog.Domain.Products;
using Ecommerce.Services.Catalog.Domain.Products.Specifications;
using MapsterMapper;
using MediatR;

namespace Ecommerce.Services.Catalog.Application.Features.Products.Queries;

public class GetVariantsByIdsCommandHandler(IEfUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetVariantsByIdsQuery, Result<List<VariantDto>>>
{
    public async Task<Result<List<VariantDto>>> Handle(GetVariantsByIdsQuery request, CancellationToken cancellationToken)
    {
        var productVariantRepository = unitOfWork.Repository<ProductVariant, Guid>();
        
        var spec = new VariantsAndOptionsSpec(request.VariantIds);

        var productVariants = await productVariantRepository.GetListAsync(spec, cancellationToken: cancellationToken);

        var result = mapper.Map<List<VariantDto>>(productVariants);

        return Result<List<VariantDto>>.Success(result);
    }
}