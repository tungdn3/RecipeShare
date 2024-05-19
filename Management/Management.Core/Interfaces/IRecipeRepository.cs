using Management.Core.Entities;

namespace Management.Core.Interfaces;

public interface IRecipeRepository
{
    Task<int> Create(Recipe recipe);
    
    Task<Recipe?> GetById(int id);
    
    Task<Recipe[]> GetByUserId(string userId, string? title);
    
    Task Update(Recipe recipe);
}
