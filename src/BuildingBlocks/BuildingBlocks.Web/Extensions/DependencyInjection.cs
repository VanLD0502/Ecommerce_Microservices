using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocksWeb(this IServiceCollection services)
    {
        return services;
    }
}
