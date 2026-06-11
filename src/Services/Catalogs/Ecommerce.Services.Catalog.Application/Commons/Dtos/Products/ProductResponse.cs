namespace Ecommerce.Services.Catalog.Application.Commons.Dtos.Products;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stocks { get; set; }
}