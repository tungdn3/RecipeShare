using Elastic.Clients.Elasticsearch;
using Search.API;
using Search.API.Extensions;
using Search.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

builder.Services.AddServiceBus(builder.Configuration);
builder.Services.AddElasticSearch(builder.Configuration);
builder.Services.AddBlobStorage(builder.Configuration);
builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowFE");

app.MapControllers();

EnsureElasticSearchIndexCreated(app);

app.Run();

static void EnsureElasticSearchIndexCreated(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    try
    {
        ElasticsearchClient client = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
        ElasticSearchInitializer.EnsureIndexCreated(client, SearchConstants.ElasticSearch.IndexName);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating ElasticSearch index.");
    }
}