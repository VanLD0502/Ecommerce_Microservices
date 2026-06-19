using BuildingBlocks.Shared.Domains.Interfaces;
using System.Collections.Generic;

namespace Ecommerce.Services.Catalog.Domain.Products.Rules;

public class ProductActiveHasAtLeastOneVariantRule(IReadOnlyCollection<ProductVariant> variants) : IBusinessRule
{
    public bool IsBroken()
    {
        return variants.Count == 0;
    }

    public string ErrorMessage { get; } = "Sản phẩm muốn đăng lên phải có ít nhất 1 biến thể";
}
