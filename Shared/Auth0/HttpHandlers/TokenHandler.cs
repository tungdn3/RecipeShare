using Shared.Auth0.Exceptions;
using Shared.Auth0.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Shared.Auth0.HttpHandlers;

internal class TokenHandler : DelegatingHandler
{
    private const int ExpirationOffsetInSeconds = 300;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Auth0ClientOptions _options;
    private TokenResponse? _tokenResponse;

    public TokenHandler(IHttpClientFactory httpClientFactory, Auth0ClientOptions options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string token = await GetToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetToken()
    {
        if (_tokenResponse == null || _tokenResponse.IsExpired)
        {
            _tokenResponse = await GetNewToken();
        }

        return _tokenResponse.AccessToken;
    }

    private async Task<TokenResponse> GetNewToken()
    {
        var payload = new
        {
            client_id = _options.ClientId,
            client_secret = _options.ClientSecret,
            audience = $"{_options.BaseUrl}/api/v2/",
            grant_type = "client_credentials"
        };

        var json = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, Application.Json);

        var httpClient = _httpClientFactory.CreateClient("Auth0");
        using var httpResponseMessage = await httpClient.PostAsync("/oauth/token", json);

        httpResponseMessage.EnsureSuccessStatusCode();

        TokenResponse? response = await httpResponseMessage.Content.ReadFromJsonAsync<TokenResponse>()
            ?? throw new IntegrationException($"Failed to get access token. client_id={payload.client_id}, audience={payload.audience}, grant_type={payload.grant_type}.");

        response.ExpiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresInSeconds - ExpirationOffsetInSeconds);
        return response;
    }
}
