namespace Ecommerce.Services.Carts.Api.Models.Dtos;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}