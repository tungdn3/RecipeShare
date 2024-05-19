using Management.Core.Dtos;
using Management.Core.Entities;

namespace Management.Core.Extensions;

public static class RecipeExtensions
{
    public static RecipeDto ToDto(this Recipe recipe, string? imageUrl)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            CategoryId = recipe.CategoryId,
            CookingMinutes = recipe.CookingMinutes,
            CreatedAt = recipe.CreatedAt,
            Description = recipe.Description,
            ImageUrl = imageUrl,
            ImageFileName = recipe.ImageFileName,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            IsDeleted = recipe.IsDeleted,
            IsPublished = recipe.IsPublished,
            PreparationMinutes = recipe.PreparationMinutes,
            PublishedAt = recipe.PublishedAt,
            Title = recipe.Title,
            UpdatedAt = recipe.UpdatedAt,
            UserId = recipe.UserId,
        };
    }
}
