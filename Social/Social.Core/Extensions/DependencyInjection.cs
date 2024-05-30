using Microsoft.Extensions.DependencyInjection;
using Social.Core.Services;

namespace Social.Core.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<LikeService>();
        services.AddScoped<CommentService>();

        return services;
    }
}
