using Management.Core.Dtos;
using Management.Core.Entities;

namespace Management.Core.Interfaces;

public interface IRecipeRepository
{
    Task<int> Create(Recipe recipe);
    
    Task<Recipe?> GetById(int id);
    
    Task<PageResultDto<Recipe>> GetByUserId(string userId, string? title, int pageNumber = 1, int pageSize = 10);
    
    Task Update(Recipe recipe);
}
