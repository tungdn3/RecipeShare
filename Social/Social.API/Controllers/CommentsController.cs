using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Core.Dto;
using Social.Core.Services;

namespace Social.API.Controllers;

[Authorize]
[ApiController]
[Route("social/comments")]
public class CommentsController : ControllerBase
{
    private readonly ILogger<CommentsController> _logger;
    private readonly CommentService _commentService;

    public CommentsController(ILogger<CommentsController> logger, CommentService commentService)
    {
        _logger = logger;
        _commentService = commentService;
    }

    [AllowAnonymous]
    [HttpGet(Name = "GetByRecipe")]
    public async Task<PageResultDto<CommentDto>> GetByRecipe(
        [FromQuery] int recipeId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return await _commentService.GetByRecipe(recipeId, pageNumber, pageSize);
    }

    [AllowAnonymous]
    [HttpGet("{id}/replies", Name = "GetReplies")]
    public async Task<PageResultDto<CommentDto>> GetReplies(
        [FromRoute] int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return await _commentService.GetReplies(id, pageNumber, pageSize);
    }

    [AllowAnonymous]
    [HttpPost("count", Name = "CountRecipesComments")]
    public IEnumerable<CountDto> CountRecipesComments([FromBody] List<int> ids)
    {
        return ids.Select(id => new CountDto
        {
            Id = id,
            Count = 10,
        });
    }

    [HttpPost]
    public async Task<int> Add([FromBody] CommentRequest request)
    {
        return await _commentService.Add(request);
    }

    [HttpPut("{id}")]
    public async Task Edit(int id, [FromBody] EditCommentRequest request)
    {
        await _commentService.Edit(id, request);
    }

    [HttpDelete("{id}", Name = "DeleteComment")]
    public async Task Delete(int id)
    {
        await _commentService.Delete(id);
    }
}
