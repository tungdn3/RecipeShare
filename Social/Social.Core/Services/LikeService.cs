using Social.Core.Dto;
using Social.Core.Entities;
using Social.Core.Interfaces;

namespace Social.Core.Services;

public class LikeService
{
    private readonly ILikeRepository _likeRepository;
    private readonly IUserRepository _userRepository;

    public LikeService(ILikeRepository likeRepository, IUserRepository userRepository)
    {
        _likeRepository = likeRepository;
        _userRepository = userRepository;
    }

    public async Task<int> AddLike(LikeRequest request)
    {
        var like = new Like
        {
            LikedDate = DateTime.UtcNow,
            RecipeId = request.RecipeId,
            UserId = _userRepository.GetCurrentUserId(),
        };

         await _likeRepository.Add(like);

        return like.Id;
    }

    public Task<IEnumerable<CountDto>> CountRecipeLikes(IEnumerable<int> recipeIds)
    {
        return _likeRepository.CountRecipeLikes(recipeIds);
    }

    public async Task<LikeDto?> GetRecipeLikeByCurrentUser(int recipeId)
    {
        string userId = _userRepository.GetCurrentUserId();
        List<Like> likes = await _likeRepository.Search(recipeId, userId);
        return likes.Select(x => new LikeDto
        {
            Id = x.Id
        }).FirstOrDefault();
    }

    public async Task RemoveLike(int id)
    {
        await _likeRepository.Remove(id);
    }
}
