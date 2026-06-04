using BuildingBlocks.Logging;
using BuildingBlocks.EfCore;
using Mapster;
using MapsterMapper;
using Ecommerce.Services.Catalog.Api.Extensions.Configurations;
using Ecommerce.Services.Catalog.Api.Models.Interfaces;
using Ecommerce.Services.Catalog.Api.Services;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddCustomSerilog("Product Services");

builder.Services.AddDbConfigurations(builder.Configuration);
builder.Services.AddBuildingBlocksInfrastructure(builder.Configuration);
// 1. Cấu hình Mapster
var config = TypeAdapterConfig.GlobalSettings;
// 2. Đăng ký IMapper vào DI Container
builder.Services.AddSingleton(config);

builder.Services.AddScoped<IMapper>(sp => new Mapper(config));

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddGrpc();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // <-- Thêm dòng này để bật giao diện (Mặc định đường dẫn là /scalar/v1)
}



app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.MapGet("/health", () =>
{
    var instanceName = Environment.GetEnvironmentVariable("INSTANCE_NAME") ?? "Unknown-Instance";
    
    return Results.Ok($"Product {instanceName} OK");
});

app.MapGrpcService<ProductGrpcService>();
app.MapControllers();

app.Run();