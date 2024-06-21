using Management.Core.Dtos;

namespace Management.Core.Services;

public interface IRecipeService
{
    Task<int> Create(string userId, RecipeCreateDto dto);
    
    Task<PageResultDto<RecipeDto>> Get(string userId, string? title = null, int pageNumber = 1, int pageSize = 10);

    Task<RecipeDto?> GetById(string userId, int id);

    Task Publish(RecipePublishDto dto);
    
    Task Update(int id, RecipeUpdateDto dto);
}