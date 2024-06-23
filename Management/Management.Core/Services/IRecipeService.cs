using Management.Core.Dtos;

namespace Management.Core.Services;

public interface IRecipeService
{
    Task<int> Create(RecipeCreateDto dto);
    
    Task<PageResultDto<RecipeDto>> GetByCurrentUser(string? title = null, int pageNumber = 1, int pageSize = 10);

    Task<RecipeDto?> GetById(int id);

    Task Publish(RecipePublishDto dto);
    
    Task Update(int id, RecipeUpdateDto dto);
}