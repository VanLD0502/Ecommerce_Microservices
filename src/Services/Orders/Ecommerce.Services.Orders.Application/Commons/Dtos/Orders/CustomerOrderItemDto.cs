namespace Ecommerce.Services.Orders.Application.Commons.Dtos.Orders;

public class CustomerOrderItemDto
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ProductName { get; set; }
}   