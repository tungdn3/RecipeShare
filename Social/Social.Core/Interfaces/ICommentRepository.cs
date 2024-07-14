using Social.Core.Dto;
using Social.Core.Entities;

namespace Social.Core.Interfaces;

public interface ICommentRepository
{
    Task<int> Add(Comment comment);
    Task<bool> Exists(int id);
    Task<Comment?> GetById(int id);
    Task<PageResultDto<CommentDto>> GetReplies(int commentId, int pageNumber, int pageSize);
    Task<PageResultDto<CommentDto>> GetByRecipe(int recipeId, int pageNumber, int pageSize);
    Task Delete(int id);
    Task Save(Comment comment);
    Task<List<CountDto>> CountRecipesComments(List<int> recipeIds);
}
