namespace Social.Core.Dto;

public class CommentDto
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string UserDisplayName { get; set; } = string.Empty;

    public string? UserAvatarUrl { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int ReplyCount { get; set; }

    public string Path { get; set; } = string.Empty;
}
