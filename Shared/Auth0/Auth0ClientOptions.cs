namespace Shared.Auth0;

public class Auth0ClientOptions
{
    public string BaseUrl { get; set; } = string.Empty;

    public int MaxRetries { get; set; } = 3;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}
