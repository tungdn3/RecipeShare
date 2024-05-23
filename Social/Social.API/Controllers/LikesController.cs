using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Dto;
using Social.Core.Services;

namespace Social.API.Controllers;

[Authorize]
[ApiController]
[Route("social/likes")]
public class LikesController : ControllerBase
{
    private readonly ILogger<LikesController> _logger;
    private readonly LikeService _likeService;

    public LikesController(ILogger<LikesController> logger, LikeService likeService)
    {
        _logger = logger;
        _likeService = likeService;
    }

    [HttpGet(Name = "GetLike")]
    public async Task<LikeDto?> GetLike([FromQuery] int recipeId)
    {
        LikeDto? like = await _likeService.GetRecipeLikeByCurrentUser(recipeId);
        return like;
    }

    [AllowAnonymous]
    [HttpPost("count", Name = "CountRecipeLikes")]
    public async Task<IEnumerable<CountDto>> CountRecipesLikes([FromBody] List<int> ids)
    {
        IEnumerable<CountDto> counts = await _likeService.CountRecipeLikes(ids);
        return counts;
    }

    [HttpPost(Name = "Like")]
    public async Task<int> Like([FromBody] LikeRequest request)
    {
        int id = await _likeService.AddLike(request);
        return id;
    }

    [HttpDelete("{id}", Name = "RemoveLike")]
    public async Task RemoveLike(int id)
    {
        await _likeService.RemoveLike(id);
    }
}
