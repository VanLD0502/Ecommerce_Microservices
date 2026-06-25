using System.Text.Json.Serialization;

namespace Ecommerce.ApiGateway.Dtos;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}