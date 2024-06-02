using Notification.Api.Data.Entities;
using System.Text.Json.Serialization;

namespace Notification.Api.Dtos;

public class NotificationMessageDto
{
    public long Id { get; set; }

    public bool IsSeen { get; set; }

    public DateTime? SeenAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public int RecipeId { get; set; }

    public int? ParentCommentId { get; set; }

    public int? CommentId { get; set; }

    public string FromUserId { get; set; } = string.Empty;

    public string FromUserDisplayName { get; set; } = string.Empty;

    public string ToUserId { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NotificationType Type { get; set; }
}
