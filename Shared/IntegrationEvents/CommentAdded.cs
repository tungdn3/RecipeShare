namespace Shared.IntegrationEvents;

public class CommentAdded
{
    public int RecipeId { get; set; }

    public string UserId { get; set; } = string.Empty;
    
    public DateTime CommentedAt { get; set; }

    public int CommentId { get; set; }

    public int? ParentCommentId { get; set; }
}
