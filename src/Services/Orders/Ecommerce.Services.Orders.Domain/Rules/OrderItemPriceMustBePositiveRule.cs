using BuildingBlocks.Shared.Domains.Interfaces;

namespace Ecommerce.Services.Orders.Domain.Rules;

public class OrderItemPriceMustBePositiveRule(decimal price) : IBusinessRule
{
    public bool IsBroken()
    {
        return price <= 0;
    }

    public string ErrorMessage =>  "Order item price must be positive";
}