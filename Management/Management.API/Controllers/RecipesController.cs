using Management.Core.Commands;
using Management.Core.Dtos;
using Management.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers;

[Authorize]
[ApiController]
[Route("management/recipes")]
public class RecipesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecipesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(Name = "GetByCurrentUser")]
    public async Task<PageResultDto<RecipeDto>> GetByCurrentUser(string? title = null, int pageNumber = 1, int pageSize = 10)
    {
        PageResultDto<RecipeDto> result = await _mediator.Send(new GetRecipesByCurrentUser.GetRecipesByCurrentUserRequest
        {
            Title = title,
            PageNumber = pageNumber,
            PageSize = pageSize,
        });

        return result;
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<RecipeDto> GetById(int id)
    {
        RecipeDto recipe = await _mediator.Send(new GetRecipeDetails.GetRecipeDetailsRequest
        {
            Id = id,
        });

        return recipe;
    }

    [HttpPost(Name = "CreateRecipe")]
    public async Task<int> Create([FromBody] CreateRecipe.CreateRecipeRequest request)
    {
        int recipeId = await _mediator.Send(request);
        return recipeId;
    }

    [HttpPost("publish", Name = "PublishRecipe")]
    public async Task<IActionResult> Publish([FromBody] RecipePublishDto dto)
    {
        await _mediator.Send(new PublishRecipe.PublishRecipeRequest
        {
            Id = dto.Id,
        });

        return NoContent();
    }

    [HttpPut("{id}", Name = "UpdateRecipe")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRecipe.UpdateRecipeRequest request)
    {
        request.Id = id;
        await _mediator.Send(request);
        return NoContent();
    }
}
