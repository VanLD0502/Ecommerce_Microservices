using Ecommerce.Services.Catalog.Api.Infrastructure;
using Ecommerce.Services.Catalog.Api.Settings;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Catalog.Api.Extensions.Configurations;

public static class DbConfigurations
{
    public static IServiceCollection AddDbConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        var dbsettings = configuration.GetSection("DbSettings").Get<DbSettings>();
        services.AddSingleton(dbsettings);
        
        var serverVersion = new MySqlServerVersion(new Version(dbsettings.ServerVersion));
        services.AddDbContext<ProductDbContext>(options =>
            options.UseMySql(
                connectionString, 
                serverVersion // Tự động phát hiện phiên bản MySQL đang chạy trong Docker
            ));
        
        return services;   
    }
}