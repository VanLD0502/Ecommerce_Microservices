using BuildingBlocks.Logging;
using BuildingBlocks.EfCore;
using BuildingBlocks.Messaging;
using Ecommerce.Services.Orders.Infrastructure;
using BuildingBlocks.Web.Extensions;
using Mapster;
using MapsterMapper;
using Scalar.AspNetCore;
using BuildingBlocks.Grpc.Services;
using Ecommerce.Services.Orders.Application;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.AddCustomSerilog("Order Services");

// OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddControllers();


builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBuildingBlocksInfrastructure(builder.Configuration);
builder.Services.AddBuildingBlocksWeb();
builder.Services.AddApplicationServices();

// gRPC Clients
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
builder.Services.AddGrpcClient<ProductGrpc.ProductGrpcClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Services:ProductGrpcUrl"] ?? throw new InvalidOperationException("ProductGrpcUrl is missing."));
});
builder.Services.AddGrpcClient<CartGrpc.CartGrpcClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Services:CartGrpcUrl"] ?? throw new InvalidOperationException("CartGrpcUrl is missing."));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();
