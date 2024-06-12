using Shared.Auth0.Models;
using System.Net.Http.Json;

namespace Shared.Auth0;

public interface IAuth0Client
{
    Task<UserResponse?> GetUser(string userId);
}

public class Auth0Client : IAuth0Client
{
    private readonly HttpClient _httpClient;

    public Auth0Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserResponse?> GetUser(string userId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/api/v2/users/{userId}");
        response.EnsureSuccessStatusCode();
        UserResponse? user = await response.Content.ReadFromJsonAsync<UserResponse>();
        return user;
    }
}
