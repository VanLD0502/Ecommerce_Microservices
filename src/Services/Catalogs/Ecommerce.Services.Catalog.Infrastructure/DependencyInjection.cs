using BuildingBlocks.EfCore.Persistence.Commons;
using BuildingBlocks.Shared.InfrastructureInterfaces.Persistence.EFCore;
using Ecommerce.Services.Catalog.Infrastructure.Persistence;
using Ecommerce.Services.Orders.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Services.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        var serverVersionString = configuration["DbSettings:ServerVersion"] ?? "8.0.32";
        var serverVersion = new MySqlServerVersion(new Version(serverVersionString));

        services.AddDbContext<ProductDbContext>(options =>
            options.UseMySql(
                connectionString, 
                serverVersion
            ));

        services.AddScoped<IEfUnitOfWork, EfUnitOfWork<ProductDbContext>>();

        return services;
    }
}
