using Management.Core.Dtos;
using Management.Core.Entities;

namespace Management.Core.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetById(int id);

    Task<Category?> GetByName(string name);

    Task<Category[]> Get(string? name = null);

    Task<int> Create(CategoryCreateDto dto);
    
    Task Update(Category model);
    
    Task<bool> CheckCategoryHavingRecipes(int id);
    
    Task<bool> Exists(int id);
}
