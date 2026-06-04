using BuildingBlocks.EfCore.Persistence.Commons;
using BuildingBlocks.Shared.Abstractions.Persistence.EFCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.EfCore;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocksInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IEfUnitOfWork<>), typeof(EfUnitOfWork<>));
        return services;
    }
}