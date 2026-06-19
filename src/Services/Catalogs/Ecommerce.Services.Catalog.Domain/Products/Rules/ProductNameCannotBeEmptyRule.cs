using BuildingBlocks.Shared.Domains.Interfaces;

namespace Ecommerce.Services.Catalog.Domain.Products.Rules;

public class ProductNameCannotBeEmptyRule(string name) : IBusinessRule
{
    public bool IsBroken()
    {
        return string.IsNullOrWhiteSpace(name);
    }

    public string ErrorMessage => "Tên sản phẩm không được để trống";
}
