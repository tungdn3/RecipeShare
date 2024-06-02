namespace Notification.Api.Data;

public static class DbInitializer
{
    public static void Initialize(NotificationDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        context.Users.Add(new Entities.User
        {
            Id = "auth0|6605389a095367b2377bdd43",
            DisplayName = "Tdev"
        });
        
        context.Users.Add(new Entities.User
        {
            Id = "auth0|665c3da5f78aa99994fb2f2a",
            DisplayName = "Bob"
        });

        context.SaveChanges();

        context.UserRecipes.Add(new Entities.UserRecipe
        {
            RecipeId = 1,
            UserId = "auth0|6605389a095367b2377bdd43",
        });

        context.SaveChanges();
    }
}
