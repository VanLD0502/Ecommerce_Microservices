using Ecommerce.Services.Identity.Api.Models.Entities;
using Identity.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer;

namespace Ecommerce.Services.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize(AuthenticationSchemes = IdentityServerConstants.LocalApi.AuthenticationScheme)]
public class AuthController(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<long>> roleManager,
    Ecommerce.Services.Identity.Api.Persistances.AppDbContext dbContext) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "Email này đã được đăng ký!"
            });
        }

        var newUser = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true // Bỏ qua bước xác thực email cho đơn giản
        };

        // CreateAsync tự động băm mật khẩu bằng bcrypt trước khi lưu vào DB
        // Ta không bao giờ lưu mật khẩu dạng plain text
        var result = await userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = $"Đăng ký thất bại: {errors}"
            });
        }

        // Gán role mặc định là "Customer" cho user mới đăng ký
        await userManager.AddToRoleAsync(newUser, "Customer");

        return Ok(new RegisterResponse
        {
            Success = true,
            Message = "Đăng ký tài khoản thành công!",
            UserId = newUser.Id
        });
    }

    // API lấy thông tin user đang đăng nhập (cần đính kèm JWT Token)
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.FirstName,
            user.LastName,
            user.AvatarUrl,
            Roles = await userManager.GetRolesAsync(user)
        });
    }

    // 1. GET /api/account/users (Admin only, paginated)
    [HttpGet("users")]
    [Authorize(AuthenticationSchemes = IdentityServerConstants.LocalApi.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<UserListResponse>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var query = userManager.Users;

        if (!string.IsNullOrEmpty(search))
        {
            var searchUpper = search.ToUpper();
            query = query.Where(u => (u.Email != null && u.Email.ToUpper().Contains(searchUpper)) ||
                                     (u.FirstName != null && u.FirstName.ToUpper().Contains(searchUpper)) ||
                                     (u.LastName != null && u.LastName.ToUpper().Contains(searchUpper)));
        }

        var totalCount = await query.CountAsync();
        
        var users = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var userIds = users.Select(u => u.Id).ToList();
        var userRoles = await dbContext.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
            .ToListAsync();

        var userRolesLookup = userRoles
            .GroupBy(ur => ur.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Name ?? "").ToList());

        var userDtos = new List<UserDto>();
        var now = DateTimeOffset.UtcNow;
        foreach (var u in users)
        {
            var roles = userRolesLookup.TryGetValue(u.Id, out var r) ? r : new List<string>();
            var isLockedOut = u.LockoutEnabled && u.LockoutEnd.HasValue && u.LockoutEnd.Value > now;

            userDtos.Add(new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                AvatarUrl = u.AvatarUrl,
                Roles = roles,
                IsLockedOut = isLockedOut
            });
        }

        return Ok(new UserListResponse
        {
            Items = userDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    // 2. GET /api/account/users/{id} (Admin only)
    [HttpGet("users/{id:long}")]
    [Authorize(AuthenticationSchemes = IdentityServerConstants.LocalApi.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<UserDetailResponse>> GetUserById(long id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("Không tìm thấy người dùng!");

        return Ok(new UserDetailResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AvatarUrl = user.AvatarUrl,
            Roles = await userManager.GetRolesAsync(user),
            IsLockedOut = await userManager.IsLockedOutAsync(user),
            LockoutEnd = user.LockoutEnd
        });
    }

    // 3. PUT /api/account/users/{id} (Admin or Self)
    [HttpPut("users/{id:long}")]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateProfileRequest request)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        var isAdmin = User.IsInRole("Admin");

        if (currentUserId != id.ToString() && !isAdmin)
        {
            return Forbid("Bạn không có quyền cập nhật tài khoản này!");
        }

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("Không tìm thấy người dùng!");

        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.AvatarUrl = request.AvatarUrl ?? user.AvatarUrl;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Ok("Cập nhật thông tin thành công!");
    }

    // 4. PUT /api/account/users/{id}/roles (Admin only)
    [HttpPut("users/{id:long}/roles")]
    [Authorize(AuthenticationSchemes = IdentityServerConstants.LocalApi.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> AssignRole(long id, [FromBody] AssignRoleRequest request)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("Không tìm thấy người dùng!");

        var roleExists = await roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists) return BadRequest("Role không hợp lệ!");

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRoleAsync(user, request.RoleName);

        return Ok($"Đã gán role '{request.RoleName}' cho người dùng!");
    }

    // 5. POST /api/account/users/{id}/lock (Admin only)
    [HttpPost("users/{id:long}/lock")]
    [Authorize(AuthenticationSchemes = IdentityServerConstants.LocalApi.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> LockUser(long id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("Không tìm thấy người dùng!");

        // Khóa tài khoản vĩnh viễn (hoặc đặt thời gian rất dài)
        var result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        if (!result.Succeeded)
        {
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Ok("Đã khóa tài khoản thành công!");
    }

    // 6. POST /api/account/users/{id}/unlock (Admin only)
    [HttpPost("users/{id:long}/unlock")]
    [Authorize(AuthenticationSchemes = IdentityServerConstants.LocalApi.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> UnlockUser(long id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("Không tìm thấy người dùng!");

        var result = await userManager.SetLockoutEndDateAsync(user, null);
        if (!result.Succeeded)
        {
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Ok("Đã mở khóa tài khoản thành công!");
    }

    // 7. POST /api/account/change-password (Authenticated)
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        var user = await userManager.FindByIdAsync(currentUserId!);
        if (user == null) return NotFound();

        var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Ok("Đổi mật khẩu thành công!");
    }

    // 8. PUT /api/account/profile (Authenticated, self update)
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        var user = await userManager.FindByIdAsync(currentUserId!);
        if (user == null) return NotFound();

        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.AvatarUrl = request.AvatarUrl ?? user.AvatarUrl;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Ok("Cập nhật thông tin cá nhân thành công!");
    }
}
