using Ecommerce.Services.Orders.Domain;

namespace Ecommerce.Services.Orders.Application.Commons.Dtos.Orders;

public class CustomerOrderResponse
{
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalPrice { get; set; }
    
    public DateTimeOffset OrderDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    
    public ICollection<CustomerOrderItemDto> OrderItems { get; set; } = new List<CustomerOrderItemDto>();
    
}