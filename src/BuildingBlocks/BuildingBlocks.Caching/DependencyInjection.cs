using Contracts.Abstractions.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BuildingBlocks.Caching;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomCaching(this IServiceCollection services, string? redisConnectionString)
    {
        if (redisConnectionString == null)
        {
            throw new ArgumentNullException(nameof(redisConnectionString));
        }
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<ICacheService, RedisCacheService>();
        return services;
    }
}