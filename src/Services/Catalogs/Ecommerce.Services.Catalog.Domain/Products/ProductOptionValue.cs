using BuildingBlocks.Shared.Domains;
using System;

namespace Ecommerce.Services.Catalog.Domain.Products;

public class ProductOptionValue : EntityTrackingBase<Guid>
{
    public Guid OptionId { get; private set; }
    public string Value { get; private set; }
    public string? ImageUrl { get; private set; }
    public int SortOrder { get; private set; }

    private ProductOptionValue() { Value = null!; }

    public ProductOptionValue(Guid optionId, string value, int sortOrder)
    {
        Id = Guid.NewGuid();
        OptionId = optionId;
        Value = value;
        SortOrder = sortOrder;
    }

    public void Update(string value, int sortOrder)
    {
        Value = value;
        SortOrder = sortOrder;
    }
}
