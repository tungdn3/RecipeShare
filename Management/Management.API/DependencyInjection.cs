using FluentValidation;
using Management.Core.Interfaces;
using Management.Core.Services;
using Management.Core.Validators;
using Management.Infrastructure;
using Management.Infrastructure.EF;
using Management.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Management.API;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<CategoryCreateDtoValidator>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IImageService, ImageService>();

        return services;
    }

    // Consider moving to the Infrastructure
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        if (isDevelopment)
        {
            services.AddDbContext<ManagementDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("SchoolContextSQLite")));
        }
        else
        {
            services.AddDbContext<ManagementDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("SchoolContextMSSQL")));
        }

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddBlobImageRepository(options =>
        {
            options.ConnectionString = configuration["AzureStorageOptions:ConnectionString"]!;
            options.ImageBlobContainerName = configuration["AzureStorageOptions:ImageBlobContainerName"]!;
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
