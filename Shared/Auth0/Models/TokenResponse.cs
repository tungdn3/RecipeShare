using System.Text.Json.Serialization;

namespace Shared.Auth0.Models;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresInSeconds { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;
}
