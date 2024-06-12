using System.Text.Json.Serialization;

namespace Shared.Auth0.Models;

public class UserResponse
{
    /// <summary>
    /// String. (unique) User's unique identifier.
    /// </summary>
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Optional string. (unique) User's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Optional string. User's full name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Optional string. URL pointing to the user's profile picture.
    /// </summary>
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }

    /// <summary>
    /// Optional string. User's family name.
    /// </summary>
    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }

    /// <summary>
    /// Optional string. User's given name.
    /// </summary>
    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }

    /// <summary>
    /// Optional string. (unique) User's username.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
