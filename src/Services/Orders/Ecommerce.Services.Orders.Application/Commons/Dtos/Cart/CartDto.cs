namespace Ecommerce.Services.Orders.Application.Commons.Dtos.Cart;

public class CartDto
{
    public long CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
}