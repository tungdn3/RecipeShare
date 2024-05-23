using Microsoft.AspNetCore.Mvc;
using Social.Core.Dto;
using Social.Core.Services;

namespace Social.API.Controllers;

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

    [HttpPost("recipes/count-comments", Name = "CountRecipesComments")]
    public IEnumerable<CountDto> CountRecipesComments([FromBody] List<int> ids)
    {
        return ids.Select(id => new CountDto
        {
            Id = id,
            Count = 10,
        });
    }
}
