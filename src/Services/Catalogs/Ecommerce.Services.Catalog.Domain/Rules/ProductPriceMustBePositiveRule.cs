using BuildingBlocks.Shared.Domains.Interfaces;

namespace Ecommerce.Services.Catalog.Domain.Rules;

public class ProductPriceMustBePositiveRule(decimal price) : IBusinessRule
{
    public bool IsBroken()
    {
        return price <= 0;
    }

    public string ErrorMessage => "Giá sản phẩm phải lớn hơn 0";
}
