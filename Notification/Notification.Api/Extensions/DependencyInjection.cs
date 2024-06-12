using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Notification.Api.Consumers;
using Notification.Api.Data;
using Notification.Api.Repositories;
using Notification.Api.Services;
using System.Security.Claims;
using Shared.Auth0;

namespace Notification.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<NotificationService>();
        services.AddScoped<RecipeService>();
        services.AddScoped<UserRepository>();
        services.AddHttpContextAccessor();
        services.AddDbContext<NotificationDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DbContextSQLite")));
        services.AddServiceBusConsumers(configuration);
        services.AddAuth0Client(options =>
        {
            options.ClientId = configuration.GetValue<string>("Auth0:ClientId")!;
            options.ClientSecret = configuration.GetValue<string>("Auth0:ClientSecret")!;
            options.BaseUrl = $"https://{configuration.GetValue<string>("Auth0:Domain")}";
        });

        return services;
    }

    public static IServiceCollection AddOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Notification API",
                Description = "A Web API for managing notifications",
            });

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Notification.Api.xml"));

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"https://{configuration["Auth0:Domain"]}/authorize", UriKind.Absolute),
                    },
                },
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2",
                        },
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static WebApplication UseOpenApi(this WebApplication app, IConfiguration configuration)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.OAuthClientId(configuration["Swagger:Auth0:ClientId"]);
            c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
            {
                { "audience", configuration["Auth0:Audience"]! },
            });
        });

        return app;
    }

    public static IServiceCollection AddRecipeAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{configuration["Auth0:Domain"]}/";
                options.Audience = configuration["Auth0:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

        return services;
    }

    public static IServiceCollection AddServiceBusConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMassTransit(x =>
        {
            x.AddServiceBusMessageScheduler();

            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<LikeAddedConsumer>();
            x.AddConsumer<CommentAddedConsumer>();
            x.AddConsumer<RecipePublishedConsumer>();

            x.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("AzureServiceBus"));

                cfg.UseServiceBusMessageScheduler();

                cfg.SubscriptionEndpoint("notification", "recipe-published", e =>
                {
                    e.ConfigureConsumer<RecipePublishedConsumer>(context);
                });

                cfg.SubscriptionEndpoint("notification", "like-added", e =>
                {
                    e.ConfigureConsumer<LikeAddedConsumer>(context);
                });

                cfg.SubscriptionEndpoint("notification", "comment-added", e =>
                {
                    e.ConfigureConsumer<CommentAddedConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
