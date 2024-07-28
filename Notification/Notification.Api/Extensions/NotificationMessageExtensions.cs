using Notification.Api.Data.Entities;
using Notification.Api.Dtos;

namespace Notification.Api.Extensions;

public static class NotificationMessageExtensions
{
    public static NotificationMessageDto ToDto(this NotificationMessage message, User fromUser)
    {
        return new NotificationMessageDto
        {
            CommentId = message.CommentId,
            CreatedAt = message.CreatedAt,
            Id = message.Id,
            IsSeen = message.IsSeen,
            ParentCommentId = message.ParentCommentId,
            RecipeId = message.RecipeId,
            SeenAt = message.SeenAt,
            Type = message.Type.ToString(),
            FromUserDisplayName = fromUser.DisplayName,
            FromUserId = message.FromUserId,
            ToUserId = message.ToUserId,
        };
    }
}
