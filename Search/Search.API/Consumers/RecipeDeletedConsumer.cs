using Elastic.Clients.Elasticsearch;
using MassTransit;
using Search.API.Models;
using Shared.IntegrationEvents;

namespace Search.API.Consumers;

public class RecipeDeletedConsumer : IConsumer<RecipeDeleted>
{
    private readonly ILogger<RecipeDeletedConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public RecipeDeletedConsumer(ILogger<RecipeDeletedConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<RecipeDeleted> context)
    {
        _logger.LogInformation("Received recipe deleted {Id}", context.Message.Id);
        DeleteResponse response = await _client.DeleteAsync<Recipe>(context.Message.Id);
        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to delete Recipe Id {RecipeId}. Debug info: {DebugInfo}", context.Message.Id, response.DebugInformation);
        }
    }
}
