using Management.Core.Interfaces;
using Management.Infrastructure.AzureStorage;
using Management.Infrastructure.EF.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Management.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBlobImageRepository(this IServiceCollection services, Action<AzureStorageOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IImageRepository, BlobImageRepository>();

        return services;
    }

    public static IServiceCollection AddUserRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
