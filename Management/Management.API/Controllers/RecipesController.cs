using Management.Core.Dtos;
using Management.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers;

[Authorize]
[ApiController]
[Route("management/recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet(Name = "GetRecipes")]
    public async Task<PageResultDto<RecipeDto>> Get(string? title = null, int pageNumber = 1, int pageSize = 10)
    {
        PageResultDto<RecipeDto> result = await _recipeService.GetByCurrentUser(title, pageNumber, pageSize);
        return result;
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<RecipeDto?> GetById(int id)
    {
        RecipeDto? recipe = await _recipeService.GetById(id);
        return recipe;
    }

    [HttpPost(Name = "CreateRecipe")]
    public async Task<int> Create([FromBody] RecipeCreateDto dto)
    {
        int recipeId = await _recipeService.Create(dto);
        return recipeId;
    }

    [HttpPost("publish", Name = "PublishRecipe")]
    public async Task<IActionResult> Publish([FromBody] RecipePublishDto dto)
    {
        await _recipeService.Publish(dto);
        return NoContent();
    }

    [HttpPut("{id}", Name = "UpdateRecipe")]
    public async Task<IActionResult> Update(int id, [FromBody] RecipeUpdateDto dto)
    {
        dto.Id = id;
        await _recipeService.Update(id, dto);
        return NoContent();
    }
}
