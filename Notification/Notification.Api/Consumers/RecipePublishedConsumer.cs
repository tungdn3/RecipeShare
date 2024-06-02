using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notification.Api.Data;
using Notification.Api.Data.Entities;
using Shared.IntegrationEvents;

namespace Notification.Api.Consumers;

public class RecipePublishedConsumer : IConsumer<RecipePublished>
{
    private readonly NotificationDbContext _dbContext;

    public RecipePublishedConsumer(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<RecipePublished> context)
    {
        RecipePublished message = context.Message;

        bool userExist = await _dbContext.Users.AnyAsync(x => x.Id == message.UserId);
        if (!userExist)
        {
            throw new InvalidOperationException($"Could not find a user with the given ID '{message.UserId}'.");
        }

        UserRecipe? userRecipe = await _dbContext.UserRecipes.FirstOrDefaultAsync(x => x.RecipeId == message.Id);
        if (userRecipe == null)
        {
            _dbContext.UserRecipes.Add(new UserRecipe
            {
                RecipeId = message.Id,
                UserId = message.UserId,
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
