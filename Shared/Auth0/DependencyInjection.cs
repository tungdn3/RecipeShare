using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Shared.Auth0.HttpHandlers;

namespace Shared.Auth0;

public static class DependencyInjection
{
    public static IServiceCollection AddAuth0Client(this IServiceCollection services, Action<Auth0ClientOptions> config)
    {
        var options = new Auth0ClientOptions();
        config(options);
        if (string.IsNullOrEmpty(options.BaseUrl) || options.MaxRetries <= 0)
        {
            throw new InvalidOperationException("Please review the Auth0 Client options.");
        }

        services.AddSingleton(options);
        services.AddTransient<TokenHandler>();
        services
            .AddHttpClient<IAuth0Client, Auth0Client>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            })
            .AddHttpMessageHandler<TokenHandler>()
            .AddPolicyHandler(GetRetryPolicy(options.MaxRetries));

        services
            .AddHttpClient("Auth0", client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            })
            .AddPolicyHandler(GetRetryPolicy(options.MaxRetries));

        return services;
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int maxRetries)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(maxRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
