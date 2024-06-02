using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notification.Api.Data;
using Notification.Api.Data.Entities;
using Shared.IntegrationEvents;

namespace Notification.Api.Consumers;

public class LikeAddedConsumer : IConsumer<LikeAdded>
{
    private readonly ILogger<LikeAddedConsumer> _logger;
    private readonly NotificationDbContext _dbContext;

    public LikeAddedConsumer(ILogger<LikeAddedConsumer> logger, NotificationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<LikeAdded> context)
    {
        LikeAdded message = context.Message;
        
        UserRecipe? userRecipe = await _dbContext.UserRecipes.AsNoTracking().FirstOrDefaultAsync(x => x.RecipeId == message.RecipeId) 
            ?? throw new InvalidOperationException($"Could not find recipe with the given id '{message.RecipeId}'.");

        if (message.UserId == userRecipe.UserId)
        {
            _logger.LogWarning("User likes his own recipe. {UserId}, {RecipeId}.", message.UserId, message.RecipeId);
            return;
        }

        var notificationMessage = new NotificationMessage
        {
            IsSeen = false,
            SeenAt = null,
            CommentId = null,
            CreatedAt = DateTime.UtcNow,
            ParentCommentId = null,
            RecipeId = message.RecipeId,
            Type = NotificationType.Like,
            FromUserId = message.UserId,
            ToUserId = userRecipe.UserId,
        };

        _dbContext.NotificationMessages.Add(notificationMessage);
        await _dbContext.SaveChangesAsync();
    }
}
