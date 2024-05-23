using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Social.API.Extensions;
using Social.Core.Extensions;
using Social.Infrastructure.EF;
using Social.Infrastructure.Extensions;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
    );
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

builder.Services.AddOpenApi(builder.Configuration);

builder.Services.AddCore();

bool isDevelopment = builder.Environment.IsDevelopment();
string? devDbConnectionString = builder.Configuration.GetConnectionString("DbContextSQLite");
string? prodDbConnectionString = builder.Configuration.GetConnectionString("DbContextSQL");
if ((isDevelopment && string.IsNullOrEmpty(devDbConnectionString)) || (!isDevelopment && string.IsNullOrEmpty(prodDbConnectionString)))
{
    throw new InvalidOperationException("Missing Db connection string.");
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(
    isDevelopment,
    devDbConnectionString!,
    prodDbConnectionString!);

var app = builder.Build();

app.UseOpenApi(app.Configuration);

app.UseHttpsRedirection();

app.UseCors("AllowAny");

app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SocialDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
