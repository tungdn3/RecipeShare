using Elastic.Clients.Elasticsearch;
using MassTransit;
using Search.API.Models;
using Shared.IntegrationEvents;

namespace Search.API.Consumers;

public class RecipeUpdatedConsumer : IConsumer<RecipeUpdated>
{
    private readonly ILogger<RecipeUpdatedConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public RecipeUpdatedConsumer(ILogger<RecipeUpdatedConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<RecipeUpdated> context)
    {
        _logger.LogInformation("Received recipe updated {Id}", context.Message.Id);
        var recipe = new Recipe
        {
            Id = context.Message.Id,
            CategoryId = context.Message.CategoryId,
            CookingMinutes = context.Message.CookingMinutes,
            CreatedAt = context.Message.CreatedAt,
            Description = context.Message.Description,
            ImageFileName = context.Message.ImageFileName,
            Ingredients = context.Message.Ingredients,
            Instructions = context.Message.Instructions,
            IsDeleted = context.Message.IsDeleted,
            IsPublished = context.Message.IsPublished,
            PreparationMinutes = context.Message.PreparationMinutes,
            PublishedAt = context.Message.PublishedAt,
            Title = context.Message.Title,
            UpdatedAt = context.Message.UpdatedAt,
            UserId = context.Message.UserId,
        };

        await _client.IndexAsync(recipe);
    }
}
