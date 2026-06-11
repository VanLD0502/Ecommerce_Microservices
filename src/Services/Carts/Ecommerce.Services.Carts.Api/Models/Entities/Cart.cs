using BuildingBlocks.Shared.Domains.Interfaces;
namespace Ecommerce.Services.Carts.Api.Models.Entities;

public record Cart : IDateTracking
{
    public int CustomerId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }

    public List<CartItem> Items { get; set; }
    
    public decimal TotalPrice => Items.Sum(i => i.UnitPrice * i.Quantity);
    
    public Cart(int customerId)
    {
        CustomerId = customerId;
        CreatedDate = DateTimeOffset.UtcNow;
        LastModifiedDate = DateTimeOffset.UtcNow;
        Items = [];
    }
}