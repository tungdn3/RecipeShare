using Microsoft.EntityFrameworkCore;
using Notification.Api.Data;
using Notification.Api.Data.Entities;
using Shared.IntegrationEvents;

namespace Notification.Api.Services;

public class RecipeService
{
    private readonly NotificationDbContext _dbContext;
    private readonly UserUtility _userUtility;

    public RecipeService(NotificationDbContext dbContext, UserUtility userUtility)
    {
        _dbContext = dbContext;
        _userUtility = userUtility;
    }

    public async Task AddRecipe(RecipePublished message)
    {
        await _userUtility.EnsureUserExist(message.UserId);

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
}
