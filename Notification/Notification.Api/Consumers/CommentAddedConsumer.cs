using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notification.Api.Data;
using Notification.Api.Data.Entities;
using Shared.IntegrationEvents;

namespace Notification.Api.Consumers;

public class CommentAddedConsumer : IConsumer<CommentAdded>
{
    private readonly ILogger _logger;
    private readonly NotificationDbContext _dbContext;

    public CommentAddedConsumer(ILogger<CommentAddedConsumer> logger, NotificationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CommentAdded> context)
    {
        CommentAdded message = context.Message;
        
        UserRecipe userRecipe = await _dbContext.UserRecipes.AsNoTracking().FirstOrDefaultAsync(x => x.RecipeId == message.RecipeId)
            ?? throw new InvalidOperationException($"Could not find recipe with the given id '{message.RecipeId}'.");

        if (message.UserId == userRecipe.UserId)
        {
            _logger.LogWarning("User comments on his own recipe. {UserId}, {RecipeId}.", message.UserId, message.RecipeId);
            return;
        }
        
        var notificationMessage = new NotificationMessage
        {
            IsSeen = false,
            SeenAt = null,
            CommentId = message.CommentId,
            CreatedAt = DateTime.UtcNow,
            ParentCommentId = message.ParentCommentId,
            RecipeId = message.RecipeId,
            Type = message.ParentCommentId.HasValue ? NotificationType.Reply : NotificationType.Comment,
            FromUserId = message.UserId,
            ToUserId = userRecipe.UserId,
        };

        _dbContext.NotificationMessages.Add(notificationMessage);
        await _dbContext.SaveChangesAsync();
    }
}
