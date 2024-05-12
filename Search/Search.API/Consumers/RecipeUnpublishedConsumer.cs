using Elastic.Clients.Elasticsearch;
using MassTransit;
using Search.API.Models;
using Shared.IntegrationEvents;

namespace Search.API.Consumers;

public class RecipeUnpublishedConsumer : IConsumer<RecipeUnpublished>
{
    private readonly ILogger<RecipeUnpublishedConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public RecipeUnpublishedConsumer(ILogger<RecipeUnpublishedConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<RecipeUnpublished> context)
    {
        _logger.LogInformation("Received recipe unpublished {Id}", context.Message.Id);
        DeleteResponse response = await _client.DeleteAsync<Recipe>(context.Message.Id);
        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to delete Recipe Id {RecipeId}. Debug info: {DebugInfo}", context.Message.Id, response.DebugInformation);
        }
    }
}
