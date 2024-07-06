using FluentValidation;
using Management.Core.Commands;
using Management.Core.Interfaces;
using Management.Core.MediatorPipelines;
using Management.Core.Queries;
using Management.Infrastructure;
using Management.Infrastructure.EF;
using Management.Infrastructure.EF.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.Auth0;
using Shared.IntegrationEvents;

namespace Management.API;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<CreateCategory.Validator>();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingAzureServiceBus((_, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("AzureServiceBus"));

                cfg.Message<RecipePublished>(x =>
                {
                    x.SetEntityName("recipe-published");
                });

                cfg.Message<RecipeUnpublished>(x =>
                {
                    x.SetEntityName("recipe-unpublished");
                });

                cfg.Message<RecipeUpdated>(x =>
                {
                    x.SetEntityName("recipe-updated");
                });

                cfg.Message<RecipeDeleted>(x =>
                {
                    x.SetEntityName("recipe-deleted");
                });
            });
        });

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetRecipesByCurrentUser).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }

    // Consider moving to the Infrastructure
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        if (isDevelopment)
        {
            services.AddDbContext<ManagementDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DbContextSQLite")));
        }
        else
        {
            services.AddDbContext<ManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DbContextPostgres")));
        }

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();

        string? azureStorageConnectionString = configuration["AzureStorage:ConnectionString"];
        if (string.IsNullOrEmpty(azureStorageConnectionString))
        {
            throw new ArgumentException("Missing AzureStorage:ConnectionString");
        }

        services.AddBlobImageRepository(options =>
        {
            options.ConnectionString = azureStorageConnectionString;
            options.ImageBlobContainerName = configuration["AzureStorage:ImageBlobContainerName"]!;
        });

        services.AddUserRepository();
        services.AddAuth0Client(options =>
        {
            options.ClientId = configuration.GetValue<string>("Auth0:ClientId")!;
            options.ClientSecret = configuration.GetValue<string>("Auth0:ClientSecret")!;
            options.BaseUrl = $"https://{configuration.GetValue<string>("Auth0:Domain")}";
        });

        return services;
    }

    public static void AddOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Recipe Management API",
                Description = "A Web API for managing recipes",
            });

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Management.API.xml"));
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Management.Core.xml"));

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
    }

    public static void UseOpenApi(this WebApplication app, IConfiguration configuration)
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
    }
}
