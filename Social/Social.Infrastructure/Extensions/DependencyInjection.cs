using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.IntegrationEvents;
using Social.Core.Interfaces;
using Social.Infrastructure.Auth0;
using Social.Infrastructure.EF;

namespace Social.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSocialDbContext(this IServiceCollection services,
        bool isDevelopment,
        string sqliteConnectionString)
    {
        if (isDevelopment)
        {
            services.AddDbContext<SocialDbContext>(options => options.UseSqlite(sqliteConnectionString));
        }
        else
        {
            // update later
            services.AddDbContext<SocialDbContext>(options => options.UseSqlite(sqliteConnectionString));
        }
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }

    public static IServiceCollection AddMassTransitServiceBus(this IServiceCollection services, string serviceBusConnectionString)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingAzureServiceBus((_, cfg) =>
            {
                cfg.Host(serviceBusConnectionString);

                cfg.Message<LikeAdded>(x =>
                {
                    x.SetEntityName("like-added");
                });

                cfg.Message<CommentAdded>(x =>
                {
                    x.SetEntityName("comment-added");
                });
            });
        });
        return services;
    }
}
