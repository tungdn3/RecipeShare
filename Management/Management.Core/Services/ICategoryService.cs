using Management.Core.Dtos;

namespace Management.Core.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> Get(string? name = null);

        Task<int> Create(CategoryCreateDto dto);
        
        Task Update(int id, CategoryUpdateDto dto);
        
        Task Delete(int id);
    }
}