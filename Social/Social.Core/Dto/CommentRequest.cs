namespace Social.Core.Dto;

public class CommentRequest
{
    public string Content { get; set; } = string.Empty;

    public int RecipeId { get; set; }

    public int? ParentId { get; set; }
}
