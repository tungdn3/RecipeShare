namespace Shared.IntegrationEvents;

public class LikeAdded
{
    public int LikeId { get; set; }

    public int RecipeId { get; set; }

    public string UserId { get; set; } = string.Empty;
    
    public DateTime LikedAt { get; set; }
}
