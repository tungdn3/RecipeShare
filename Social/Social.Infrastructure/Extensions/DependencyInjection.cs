using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Social.Core.Interfaces;
using Social.Infrastructure.Auth0;
using Social.Infrastructure.EF;

namespace Social.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        bool isDevelopment,
        string devDbConnectionString,
        string prodDbConnectionString)
    {
        if (isDevelopment)
        {
            services.AddDbContext<SocialDbContext>(options => options.UseSqlite(devDbConnectionString));
        }
        else
        {
            services.AddDbContext<SocialDbContext>(options => options.UseSqlite(prodDbConnectionString));
        }

        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }
}
