using Management.Core.Dtos;
using Management.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers;

[Authorize]
[ApiController]
[Route("management/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;
    private readonly IUserService _userService;

    public RecipesController(IRecipeService recipeService, IUserService userService)
    {
        _recipeService = recipeService;
        _userService = userService;
    }

    [HttpGet(Name = "GetRecipes")]
    public async Task<IEnumerable<RecipeDto>> Get(string? title = null)
    {
        int userId = await _userService.GetCurrentUserId();
        RecipeDto[] recipes = await _recipeService.Get(userId, title);
        return recipes;
    }

    [HttpPost(Name = "CreateRecipe")]
    public async Task<int> Create([FromBody] RecipeCreateDto dto)
    {
        int userId = await _userService.GetCurrentUserId();
        int recipeId = await _recipeService.Create(userId, dto);
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
