using BuildingBlocks.Caching;
using BuildingBlocks.Grpc.Services;
using BuildingBlocks.Logging;
using Ecommerce.Services.Carts.Api.Endpoints;
using Ecommerce.Services.Carts.Api.Models.Interfaces;
using Ecommerce.Services.Carts.Api.Services;
using Mapster;
using MapsterMapper;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddCustomSerilog("Cart Api");

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddCustomCaching(builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("RedisConnectionString is missing."));
//
// builder.Services.AddHttpClient<ICartProductService, CartProductService>(client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["ApiGateway:Url"] ?? "http://localhost:5001");
// });
builder.Services.AddScoped<ICartProductService, CartProductGrpcService>();
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
builder.Services.AddGrpcClient<ProductGrpc.ProductGrpcClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Services:ProductGrpcUrl"]);
});

var config = TypeAdapterConfig.GlobalSettings;
// 2. Đăng ký IMapper vào DI Container
builder.Services.AddSingleton(config);

builder.Services.AddScoped<IMapper>(sp => new Mapper(config));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.AddMappingEndpoints();

app.MapGet("/health", () =>
{
    var instanceName = Environment.GetEnvironmentVariable("INSTANCE_NAME") ?? "Unknown-Instance";
    
    return Results.Ok($"Cart {instanceName} OK");
});

app.UseHttpsRedirection();


app.Run();

