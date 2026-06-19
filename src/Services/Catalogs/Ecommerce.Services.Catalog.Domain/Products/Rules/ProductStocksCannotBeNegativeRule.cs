using BuildingBlocks.Shared.Domains.Interfaces;

namespace Ecommerce.Services.Catalog.Domain.Products.Rules;

public class ProductStocksCannotBeNegativeRule(int stocks) : IBusinessRule
{
    public bool IsBroken()
    {
        return stocks < 0;
    }

    public string ErrorMessage => "Số lượng tồn kho không được là số âm";
}
