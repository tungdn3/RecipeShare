using Microsoft.EntityFrameworkCore;
using Notification.Api.Data;
using Notification.Api.Data.Entities;
using Shared.Auth0;
using Shared.IntegrationEvents;

namespace Notification.Api.Services;

public class RecipeService
{
    private readonly NotificationDbContext _dbContext;
    private readonly IAuth0Client _auth0Client;

    public RecipeService(NotificationDbContext dbContext, IAuth0Client auth0Client)
    {
        _dbContext = dbContext;
        _auth0Client = auth0Client;
    }

    public async Task AddRecipe(RecipePublished message)
    {
        await EnsureUserExist(message.UserId);

        UserRecipe? userRecipe = await _dbContext.UserRecipes.FirstOrDefaultAsync(x => x.RecipeId == message.Id);
        if (userRecipe == null)
        {
            _dbContext.UserRecipes.Add(new UserRecipe
            {
                RecipeId = message.Id,
                UserId = message.UserId,
            });

            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task EnsureUserExist(string userId)
    {
        bool userExist = await _dbContext.Users.AnyAsync(x => x.Id == userId);
        if (userExist)
        {
            return;
        }

        var user = await _auth0Client.GetUser(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"Could not find a user with the given ID '{userId}'.");
        }

        _dbContext.Users.Add(new User
        {
            Id = userId,
            DisplayName = user.Name ?? user.Username ?? user.Email ?? user.UserId.Substring(user.UserId.Length - 6),
        });
        await _dbContext.SaveChangesAsync();
    }
}
