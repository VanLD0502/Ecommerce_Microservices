using BuildingBlocks.Shared.Entities;

namespace Ecommerce.Services.Catalog.Api.Models.Entities;

public class Product : EntityBase<Guid>
{
    public string Name { get; init; }
    public string Category { get; init; }
    public string Summary { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
}