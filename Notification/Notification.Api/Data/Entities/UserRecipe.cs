namespace Notification.Api.Data.Entities;

public class UserRecipe
{
    public string UserId { get; set; } = string.Empty;

    public int RecipeId { get; set; }

    public User User { get; set; }
}
