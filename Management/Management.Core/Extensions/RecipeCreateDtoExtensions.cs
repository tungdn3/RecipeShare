using Management.Core.Dtos;
using Management.Core.Entities;

namespace Management.Core.Extensions;

public static class RecipeCreateDtoExtensions
{
    public static Recipe ToEntity(this RecipeCreateDto dto, string userId)
    {
        DateTime now = DateTime.UtcNow;
        return new Recipe
        {
            CategoryId = dto.CategoryId,
            CookingMinutes = dto.CookingMinutes,
            Description = dto.Description,
            ImageFileName = dto.ImageFileName,
            Ingredients = dto.Ingredients,
            Instructions = dto.Instructions,
            IsPublished = dto.IsPublished,
            PreparationMinutes = dto.PreparationMinutes,
            Title = dto.Title,
            CreatedAt = now,
            IsDeleted = false,
            PublishedAt = dto.IsPublished ? now : null,
            UpdatedAt = now,
            UserId = userId,
        };
    }
}
