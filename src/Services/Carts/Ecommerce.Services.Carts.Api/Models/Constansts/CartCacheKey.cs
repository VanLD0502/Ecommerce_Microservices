namespace Ecommerce.Services.Carts.Api.Models.Constansts;

public static class CartCacheKey
{
    public static string GetCartCacheKey(int userId) => $"cart-{userId}";
    public static string GetCartCacheKey(string userId) => $"cart-{userId}";
    
}