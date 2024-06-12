using MassTransit;
using Notification.Api.Services;
using Shared.IntegrationEvents;

namespace Notification.Api.Consumers;

public class RecipePublishedConsumer : IConsumer<RecipePublished>
{
    private readonly RecipeService _recipeService;

    public RecipePublishedConsumer(RecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public Task Consume(ConsumeContext<RecipePublished> context)
    {
        return _recipeService.AddRecipe(context.Message);
    }
}
