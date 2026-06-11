using BuildingBlocks.Shared.Commons;
using BuildingBlocks.Shared.Enums;
using BuildingBlocks.Shared.InfrastructureInterfaces.Caching;
using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Ecommerce.Services.Carts.Api.Models.Constansts;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Carts.Api.Features.Carts.Commands.RemoveCart;

public record RemoveCartCommand(int CustomerId) : ICommand;

public class RemoveCartCommandHandler(
    ICacheService cacheService,
    ILogger<RemoveCartCommandHandler> logger)
    : ICommandHandler<RemoveCartCommand>
{
    public async Task<Result> Handle(RemoveCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await cacheService.RemoveAsync(
                CartCacheKey.GetCartCacheKey(request.CustomerId),
                cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi xóa giỏ hàng của khách hàng {CustomerId}: {Message}", request.CustomerId, ex.Message);
            return Result.Failure(ex.Message, EErrorCode.InternalServerError);
        }
    }
}
