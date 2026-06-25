using System.Reflection;
using BuildingBlocks.Messaging.Core;
using BuildingBlocks.Messaging.Settings;
using BuildingBlocks.Shared.InfrastructureInterfaces.Messaging;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messaging;
public static class DependencyInjection
{
    public static void AddMasstransitEventBus(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator>? configure = null)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName)); 
        
        services.AddMassTransit(x =>
        {
            x.AddConsumers(callingAssembly);
            configure?.Invoke(x);
            x.UsingRabbitMq((context, cfg) =>
            {
                
                var settings = context
                    .GetRequiredService<IOptions<RabbitMqSettings>>()
                    .Value;

                cfg.Host(
                    settings.Host,
                    settings.VirtualHost,
                    h =>
                    {
                        h.Username(settings.Username);
                        h.Password(settings.Password);
                    });
                
                //Tự động tạo queue tương ứng consumer
                cfg.ConfigureEndpoints(context);
        
                cfg.UseMessageRetry(r =>
                {
                    r.Interval(
                        retryCount: 3,
                        interval: TimeSpan.FromSeconds(5));
                });
            });
        });

        services.AddScoped<IEventPublisher, EventPublisher>();
    }
}