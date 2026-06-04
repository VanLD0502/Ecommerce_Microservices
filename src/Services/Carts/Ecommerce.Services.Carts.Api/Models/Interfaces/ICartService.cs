using BuildingBlocks.Shared.Commons;
using Ecommerce.Services.Carts.Api.Models.Dtos;
using Ecommerce.Services.Carts.Api.Models.Entities;

namespace Ecommerce.Services.Carts.Api.Models.Interfaces;

public interface ICartService
{
    public Task<Result<Cart?>> GetCart(int customerId);
    public Task<Result<CartItem>> AddItemToCart(int customerId, Guid productId, int quantity);
    public Task<Result> RemoveItemFromCart(int customerId, Guid productId);
    public Task<Result> UpdateQuantityAsync(int customerId, Guid productId, int quantity);
    public Task<Result> RemoveCart(int customerId);
}