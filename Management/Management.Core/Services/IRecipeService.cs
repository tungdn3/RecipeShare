using Management.Core.Dtos;

namespace Management.Core.Services;

public interface IRecipeService
{
    Task<int> Create(string userId, RecipeCreateDto dto);
    
    Task<RecipeDto[]> Get(string userId, string? title = null);

    Task<RecipeDto?> GetById(string userId, int id);

    Task Publish(RecipePublishDto dto);
    
    Task Update(int id, RecipeUpdateDto dto);
}