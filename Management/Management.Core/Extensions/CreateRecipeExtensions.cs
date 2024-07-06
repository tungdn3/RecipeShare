using Management.Core.Commands;
using Management.Core.Entities;

namespace Management.Core.Extensions;

public static class CreateRecipeExtensions
{
    public static Recipe ToEntity(this CreateRecipe.CreateRecipeRequest request, string userId)
    {
        DateTime now = DateTime.UtcNow;
        return new Recipe
        {
            CategoryId = request.CategoryId,
            CookingMinutes = request.CookingMinutes,
            Description = request.Description,
            ImageFileName = request.ImageFileName,
            Ingredients = request.Ingredients,
            Instructions = request.Instructions,
            IsPublished = request.IsPublished,
            PreparationMinutes = request.PreparationMinutes,
            Title = request.Title,
            CreatedAt = now,
            IsDeleted = false,
            PublishedAt = request.IsPublished ? now : null,
            UpdatedAt = now,
            UserId = userId,
        };
    }
}
