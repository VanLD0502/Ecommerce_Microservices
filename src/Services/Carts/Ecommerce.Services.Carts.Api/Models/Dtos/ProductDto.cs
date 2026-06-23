namespace Ecommerce.Services.Carts.Api.Models.Dtos;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public string ProductName { get; set; }
    public string VariantName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal AvailableStocks { get; set; }
}