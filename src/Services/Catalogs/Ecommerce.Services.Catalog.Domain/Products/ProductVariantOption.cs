using System;

namespace Ecommerce.Services.Catalog.Domain.Products;

public class ProductVariantOption
{
    public Guid VariantId { get; private set; }
    public Guid OptionValueId { get; private set; }

    public ProductVariant Variant { get; private set; } = null!;
    public ProductOptionValue OptionValue { get; private set; } = null!;

    private ProductVariantOption() { }

    public ProductVariantOption(Guid variantId, Guid optionValueId)
    {
        VariantId = variantId;
        OptionValueId = optionValueId;
    }
}
