using Duende.IdentityServer.Models;

namespace Ecommerce.Services.Identity.Api.Config;

public static class IdentityServerConfig
{
    // 1. Định nghĩa các thông tin cơ bản về User (OIDC standard)
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(), // Bắt buộc phải có theo chuẩn OIDC
            new IdentityResources.Profile(), // Trả về thông tin cá nhân cơ bản
            new IdentityResources.Email(),    // Trả về email của user
        };

    // 2. Định nghĩa các API Scopes (Quyền truy cập các service API)
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("catalog", "Catalog Service API Access"),
            new ApiScope("cart", "Cart Service API Access"),
            new ApiScope("order", "Order Service API Access"),
            new ApiScope(Duende.IdentityServer.IdentityServerConstants.LocalApi.ScopeName, "Identity Server Local API")
        };

    // 3. Định nghĩa danh sách các Client (Ứng dụng kết nối lấy Token)
    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // Client 1: Dành cho việc test API qua Postman hoặc sau này là Frontend (React, Vue)
            new Client
            {
                ClientId = "ecommerce-client",
                ClientName = "Ecommerce Web Application",
                AllowedGrantTypes = GrantTypes.Code, // Flow đăng nhập Code + PKCE bảo mật nhất
                RequirePkce = true,
                RequireClientSecret = false, // Client chạy dưới trình duyệt/mobile không tự giữ secret an toàn được
                
                RedirectUris = { "https://oauth.pstmn.io/v1/callback", "http://localhost:5173/callback" }, // Địa chỉ nhận Code
                PostLogoutRedirectUris = { "http://localhost:5173/index.html" },
                AllowedCorsOrigins = { "http://localhost:5173" },

                AllowedScopes = { "openid", "profile", "email", "catalog", "cart", "order", Duende.IdentityServer.IdentityServerConstants.LocalApi.ScopeName },
                AllowOfflineAccess = true // Hỗ trợ lấy Refresh Token
            },

            // Client 2: Dành cho các Service gọi nhau chạy ngầm (Service-to-Service)
            new Client
            {
                ClientId = "service-client",
                ClientName = "Internal Service Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials, // Chỉ cần ClientId & ClientSecret là lấy được token
                ClientSecrets = { new Secret("super-secret-key-for-internal-services".Sha256()) },
                
                AllowedScopes = { "catalog", "cart", "order" }
            },
            new Client()
            {
                ClientId = "api-client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientName = "Internal Service API Client",
                AllowedScopes = {"openid", "profile", "catalog", "cart", "order", Duende.IdentityServer.IdentityServerConstants.LocalApi.ScopeName },
                // Không cần ClientSecret vì đây là public client (React App, Mobile App)
                // Các app này chạy phía client, không giữ secret an toàn được
                RequireClientSecret = false,
                
                // Token sống 1 giờ → sau 1 giờ phải đăng nhập lại hoặc dùng Refresh Token
                AccessTokenLifetime = 3600,
                AbsoluteRefreshTokenLifetime = 7 * 3600 * 24,
                RefreshTokenUsage = TokenUsage.OneTimeOnly, // rotate token
                AllowOfflineAccess = true, // Cho phép cấp Refresh Token
            }
        };
}
