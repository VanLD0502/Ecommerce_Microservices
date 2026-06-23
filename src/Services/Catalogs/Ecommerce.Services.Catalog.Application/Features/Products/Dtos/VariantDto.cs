namespace Ecommerce.Services.Catalog.Application.Features.Products.Dtos;

public class VariantDto
{
    public Guid ProductId { get; set; }
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public int AvailableStocks { get; set; }
    public decimal Price { get; set; }
    public string VariantName { get; set; }
}