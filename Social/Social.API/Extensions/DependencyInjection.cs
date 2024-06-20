using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Shared.Auth0;
using Social.Infrastructure.Extensions;

namespace Social.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
    {
        if (environment.IsDevelopment() )
        {
            string? sqliteConnectionString = configuration.GetConnectionString("DbContextSQLite");
            if (string.IsNullOrWhiteSpace(sqliteConnectionString))
            {
                throw new InvalidOperationException("Missing SQLite connection string.");
            }

            services.AddSqlite(sqliteConnectionString);
        }
        else
        {
            string? connectionString = configuration.GetConnectionString("DbContextPostgres");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Missing Postgres connection string.");
            }

            services.AddPostgres(connectionString);
        }

        services.AddRepositories();

        string? serviceBusConnectionString = configuration.GetConnectionString("AzureServiceBus");
        if (string.IsNullOrEmpty(serviceBusConnectionString))
        {
            throw new InvalidOperationException("Missing Service Bus connection string.");
        }

        services.AddMassTransitServiceBus(serviceBusConnectionString);
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
                Title = "Social API",
                Description = "A Web API for managing likes, comments, and follows",
            });

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Social.API.xml"));
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Social.Core.xml"));

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
