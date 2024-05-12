using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using MassTransit;
using Search.API.Consumers;
using Search.API.Models;
using Search.API.Services;

namespace Search.API;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMassTransit(x =>
        {
            x.AddServiceBusMessageScheduler();

            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<RecipePublishedConsumer>();
            x.AddConsumer<RecipeUnpublishedConsumer>();
            x.AddConsumer<RecipeUpdatedConsumer>();
            x.AddConsumer<RecipeDeletedConsumer>();

            x.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("AzureServiceBus"));

                cfg.UseServiceBusMessageScheduler();

                cfg.SubscriptionEndpoint("search", "recipe-published", e =>
                {
                    e.ConfigureConsumer<RecipePublishedConsumer>(context);
                });

                cfg.SubscriptionEndpoint("search", "recipe-unpublished", e =>
                {
                    e.ConfigureConsumer<RecipeUnpublishedConsumer>(context);
                });

                cfg.SubscriptionEndpoint("search", "recipe-updated", e =>
                {
                    e.ConfigureConsumer<RecipeUpdatedConsumer>(context);
                });

                cfg.SubscriptionEndpoint("search", "recipe-deleted", e =>
                {
                    e.ConfigureConsumer<RecipeDeletedConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }

    public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSingleton(p =>
        {
            var connectionSettings = new ElasticsearchClientSettings(new Uri(configuration["ElasticSearch:Host"]!))
                .Authentication(new BasicAuthentication(configuration["ElasticSearch:UserName"]!, configuration["ElasticSearch:Password"]!))
                .DefaultMappingFor<Recipe>(i => i
                    .IndexName(SearchConstants.ElasticSearch.IndexName)
                    .IdProperty(p => p.Id)
                )
                .EnableDebugMode()
                .PrettyJson();

            var client = new ElasticsearchClient(connectionSettings);
               
            return client;
        });
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<SearchService>();
        return services;
    }
}
