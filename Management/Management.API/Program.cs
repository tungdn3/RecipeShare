using Management.API;
using Management.Infrastructure.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE", config =>
    {
        config.WithOrigins(builder.Configuration["FeUrl"]!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

//builder.Services
//  .AddAuthorization(options =>
//  {
//      options.AddPolicy(
//        "read:messages",
//        policy => policy.Requirements.Add(
//          new HasScopeRequirement("read:messages", "https://dev-recipe-share.com")
//        )
//      );
//  });

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCore(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(app.Configuration);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ManagementDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseCors("AllowFE");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
