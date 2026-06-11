using BuildingBlocks.Shared.Domains.Interfaces;

namespace Ecommerce.Services.Orders.Domain.Rules;

public class OrderItemQuantityMustBePositiveRule(int quantity) : IBusinessRule
{
    public bool IsBroken()
    {
        return quantity <= 0;
    }

    public string ErrorMessage => "Order item quantity must be positive";
}