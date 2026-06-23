namespace Ecommerce.Services.Carts.Api.Features.Carts.Dtos;

public class CartResponse
{
    public long CustomerId { get; set; }
    public List<CartItemResponse> Items { get; set; }
}