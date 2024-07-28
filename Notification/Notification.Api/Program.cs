using Notification.Api.Data;
using Notification.Api.Extensions;
using Notification.Api.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(builder.Configuration);
builder.Services.AddRecipeAuthentication(builder.Configuration);
builder.Services.AddCors(options =>
{
    string[] clientAppOrigins = builder.Configuration["ClientAppOrigins"]!.Split(";");
    options.AddPolicy("AllowClientApps", policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(clientAppOrigins)
        .AllowCredentials()
    );
});
builder.Services.AddServices(builder.Configuration, builder.Environment);
builder.Services.AddSignalR();

var app = builder.Build();

app.UseOpenApi(app.Configuration);

app.UseCors("AllowClientApps");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<NotificationHub>("/notification/hub");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<NotificationDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

app.Run();
