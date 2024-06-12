using MassTransit;
using Shared.IntegrationEvents;
using Social.Core.Dto;
using Social.Core.Entities;
using Social.Core.Interfaces;

namespace Social.Core.Services;

public class LikeService
{
    private readonly ILikeRepository _likeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public LikeService(ILikeRepository likeRepository, IUserRepository userRepository, IPublishEndpoint publishEndpoint)
    {
        _likeRepository = likeRepository;
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<int> AddLike(LikeRequest request)
    {
        string userId = await _userRepository.GetCurrentUserId();
        DateTime now = DateTime.UtcNow;
        var like = new Like
        {
            LikedDate = now,
            RecipeId = request.RecipeId,
            UserId = userId,
        };

        int id = await _likeRepository.Add(like);

        await _publishEndpoint.Publish(new LikeAdded
        {
            LikeId = id,
            LikedAt = now,
            UserId = userId,
            RecipeId = request.RecipeId,
        });

        return like.Id;
    }

    public Task<IEnumerable<CountDto>> CountRecipeLikes(IEnumerable<int> recipeIds)
    {
        return _likeRepository.CountRecipeLikes(recipeIds);
    }

    public async Task<LikeDto?> GetRecipeLikeByCurrentUser(int recipeId)
    {
        string userId = await _userRepository.GetCurrentUserId();
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
