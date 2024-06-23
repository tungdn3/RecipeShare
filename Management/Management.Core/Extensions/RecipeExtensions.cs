using Management.Core.Dtos;
using Management.Core.Entities;
using Shared.IntegrationEvents;

namespace Management.Core.Extensions;

public static class RecipeExtensions
{
    public static RecipeDto ToDto(this Recipe recipe, string? imageUrl, string? categoryName, UserDto? user)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            CategoryId = recipe.CategoryId,
            CategoryName = categoryName ?? string.Empty,
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
            User = user,
        };
    }

    public static RecipePublished ToRecipePublished(this Recipe recipe)
    {
        return new RecipePublished
        {
            Id = recipe.Id,
            CategoryId = recipe.CategoryId,
            CookingMinutes = recipe.CookingMinutes,
            CreatedAt = recipe.CreatedAt,
            Description = recipe.Description,
            ImageFileName= recipe.ImageFileName,
            Ingredients= recipe.Ingredients,
            Instructions = recipe.Instructions,
            IsDeleted = recipe.IsDeleted,
            IsPublished = recipe.IsPublished,
            PreparationMinutes= recipe.PreparationMinutes,
            PublishedAt = recipe.PublishedAt,
            Title = recipe.Title,
            UpdatedAt = recipe.UpdatedAt,
            UserId = recipe.UserId,
        };
    }

    public static RecipeUpdated ToRecipeUpdated(this Recipe recipe)
    {
        return new RecipeUpdated
        {
            Id = recipe.Id,
            CategoryId = recipe.CategoryId,
            CookingMinutes = recipe.CookingMinutes,
            CreatedAt = recipe.CreatedAt,
            Description = recipe.Description,
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
