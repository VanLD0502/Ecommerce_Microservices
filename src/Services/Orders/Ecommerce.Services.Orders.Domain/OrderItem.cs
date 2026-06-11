using BuildingBlocks.Shared.Domains;

namespace Ecommerce.Services.Orders.Domain;

public class OrderItem : EntityTrackingBase<Guid>
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ProductName { get; set; }
    
    public virtual Order Order { get; set; }
}