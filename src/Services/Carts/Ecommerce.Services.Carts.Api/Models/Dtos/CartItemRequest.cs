namespace Ecommerce.Services.Carts.Api.Models.Dtos;

public class CartItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}