using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Ecommerce.Services.Orders.Infrastructure.Persistence;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using BuildingBlocks.EfCore.Persistence.Commons;
using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Orders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMasstransitEventBus(configuration);
        
        
        services.AddScoped<IEfUnitOfWork, EfUnitOfWork<OrderDbContext>>();
        
        return services;
    }
}
