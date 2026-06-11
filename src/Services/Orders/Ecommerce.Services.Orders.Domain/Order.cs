using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Shared.Domains;
using BuildingBlocks.Shared.Domains.Interfaces;
using Ecommerce.Services.Orders.Domain.Rules;

namespace Ecommerce.Services.Orders.Domain;

public sealed class Order : AggregateRoot<Guid>, IDateTracking
{   
    public long CustomerId { get; }
    public OrderStatus Status { get; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }

    private Order() {}
    
    public decimal TotalPrice { get; private set; }
    
    public Order(long customerId)
    {
        CustomerId = customerId;
        Status = OrderStatus.AwaitingPayment;
        TotalPrice = 0;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        Check(new OrderItemPriceMustBePositiveRule(unitPrice));
        Check(new OrderItemQuantityMustBePositiveRule(quantity));

        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = this.Id,
            ProductId = productId,
            ProductName = productName,
            UnitPrice = unitPrice,
            Quantity = quantity
        };

        OrderItems.Add(item);
        TotalPrice = CalculateTotalPrice();
    }

    private decimal CalculateTotalPrice() => OrderItems.Sum(item => item.UnitPrice * item.Quantity);
}

public enum OrderStatus
{
    AwaitingPayment = 1,
}