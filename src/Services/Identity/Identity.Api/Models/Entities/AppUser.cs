using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Shared.Domains;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services.Identity.Api.Models.Entities;

public class AppUser : IdentityUser<long>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }

    [NotMapped] public string FullName => $"{FirstName} {LastName}".Trim();
    
    public DateTimeOffset CreatedDate { get; set; }
    
    //LockoutEnabled: True tức là tài khoản có thể bị khóa, False tức là không thể bị khóa (tài khoản này thường dùng cho trường hợp khẩn cấp, nếu bị block sẽ ảnh hưởng đến vận hàng hệ thống)
    public bool IsLockedOut() => LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;
}