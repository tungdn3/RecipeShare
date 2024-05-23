using Social.Core.Dto;
using Social.Core.Entities;

namespace Social.Core.Interfaces;

public interface ILikeRepository
{
    Task<int> Add(Like like);
    
    Task<IEnumerable<CountDto>> CountRecipeLikes(IEnumerable<int> recipeIds);
    
    Task<Like?> GetById(int id);
    
    Task Remove(int id);

    Task<List<Like>> Search(int? recipeId = null, string? userId = null, int top = 10);
}
