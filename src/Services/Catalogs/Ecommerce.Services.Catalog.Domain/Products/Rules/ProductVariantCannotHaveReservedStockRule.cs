using BuildingBlocks.Shared.Domains.Interfaces;

namespace Ecommerce.Services.Catalog.Domain.Products.Rules;

public class ProductVariantCannotHaveReservedStockRule(int reservedStocks) : IBusinessRule
{
    public bool IsBroken()
    {
        return reservedStocks > 0;
    }

    public string ErrorMessage { get; } = "Biến thể này hiện đang có người mua đặt chỗ (Reserved), không thể xóa";
}
