namespace Notification.Api.Data.Entities;

public class NotificationMessage
{
    public long Id { get; set; }

    public bool IsSeen { get; set; }

    public DateTime? SeenAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public int RecipeId { get; set; }

    public int? ParentCommentId { get; set; }

    public int? CommentId { get; set; }

    public string FromUserId { get; set; } = string.Empty;

    public string ToUserId { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public User FromUser { get; set; }
}
