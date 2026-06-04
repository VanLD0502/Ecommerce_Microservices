using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Services.Catalog.Api.Models.Dtos;

public class ProductRequest
{
    [MaxLength(10,  ErrorMessage = "Maximum allowed length is 10 characters.")]
    public string Name { get; set; } = String.Empty;
    public string Category { get; set; } = String.Empty;
    public string Summary { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Price { get; set; }
}