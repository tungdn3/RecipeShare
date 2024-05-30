namespace Social.Core.Entities;

public class Comment
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public string Path { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public string BuildChildrenPath()
    {
        return !string.IsNullOrEmpty(Path) ? $"{Path}.{Id}" : $"{Id}";
    }
}
