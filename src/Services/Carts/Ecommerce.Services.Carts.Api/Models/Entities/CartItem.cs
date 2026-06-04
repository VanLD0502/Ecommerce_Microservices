using BuildingBlocks.Shared.Entities.Interfaces;

namespace Ecommerce.Services.Carts.Api.Models.Entities;

public class CartItem
{
    public Guid ProductId  {get; set;}
    public decimal UnitPrice  {get; set;}
    public int Quantity   {get; set;}
    public string ProductName { get; set; }
}