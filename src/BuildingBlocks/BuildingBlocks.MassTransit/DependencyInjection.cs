using BuildingBlocks.MassTransit.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.MassTransit;
public static class DependencyInjection
{
    public static void AddMassTransit(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        // builder.Services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName)); 


        builder.Services.AddMassTransit(x =>
        {
            // x.AddConsumer<HelloMessageConsumer>()
            //     .Endpoint(e => e.Name = "abc");
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
        
                cfg.ConfigureEndpoints(context);
        
                cfg.UseMessageRetry(r =>
                {
                    r.Interval(
                        retryCount: 3,
                        interval: TimeSpan.FromSeconds(5));
                });
            });
        });

    }
}