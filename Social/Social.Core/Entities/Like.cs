namespace Social.Core.Entities;

public class Like
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime LikedDate { get; set; }
}
