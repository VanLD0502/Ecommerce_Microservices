using BuildingBlocks.Shared.InfrastructureInterfaces.InMemoryBus;
using Ecommerce.Services.Carts.Api.Features.Carts.Dtos;
using Ecommerce.Services.Carts.Api.Models.Entities;

namespace Ecommerce.Services.Carts.Api.Features.Carts.Queries.GetCart;

public record GetCartQuery(long CustomerId) : IQuery<CartResponse>;