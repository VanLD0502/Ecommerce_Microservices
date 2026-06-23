using BuildingBlocks.Shared.Domains;

namespace Ecommerce.Services.Orders.Domain;

public class OrderItem : EntityTrackingBase<Guid>
{
    public Guid OrderId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ProductName { get; set; }
    public string VariantName { get; set; }
    public Order Order { get; set; }
}