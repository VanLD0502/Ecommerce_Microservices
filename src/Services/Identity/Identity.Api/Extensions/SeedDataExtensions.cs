using Ecommerce.Services.Identity.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Identity.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedUserAndRoleAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole<long>> roleManager)
    {
        var roleCount = await roleManager.Roles.CountAsync();

        if (roleCount == 0)
        {
            foreach (var role in new[] { "Admin", "Customer", "Seller" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<long>(role));
            }
        }
        else return;
        
        var userCount = await userManager.Users.CountAsync();

        if (userCount > 0)
        {
            return;
        }
        
        // 1. Seed Admin
        var adminEmail = "vanpc1906@gmail.com";
        var adminUser = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Van",
            LastName = "PC",
            EmailConfirmed = true,
            CreatedDate = DateTimeOffset.UtcNow
        };
        
        var result = await userManager.CreateAsync(adminUser, "Password123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        
        
        // 2. Seed 10 Customers
        for (int i = 1; i <= 10; i++)
        {
            var customerEmail = $"customer{i}@gmail.com";

            var customerUser = new AppUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                FirstName = "Customer",
                LastName = $"Number {i}",
                EmailConfirmed = true,
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            var customerResult = await userManager.CreateAsync(customerUser, "Password123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customerUser, "Customer");
            }
            else 
            {
                // Lấy ra lý do tại sao lỗi và quăng ra để Rider bắt được
                var errors = string.Join(", ", customerResult.Errors.Select(e => e.Description));
                throw new Exception($"Lỗi tạo Customer {i}: {errors}");
            }
        }
    }
}