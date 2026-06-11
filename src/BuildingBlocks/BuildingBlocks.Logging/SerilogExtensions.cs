using Microsoft.AspNetCore.Builder;
using Serilog;

namespace BuildingBlocks.Logging;

public static class SerilogExtensions
{
    public static void AddCustomSerilog(this WebApplicationBuilder builder, string applicationName)
    {
        // Khởi tạo bootstrap logger trước
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            // Tự động đính kèm tên của Microservice vào log để dễ phân biệt
            .Enrich.WithProperty("ApplicationName", applicationName)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName());
    }
}