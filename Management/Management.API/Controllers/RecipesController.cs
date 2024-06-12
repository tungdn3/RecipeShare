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
    private readonly IUserService _userService;

    public RecipesController(IRecipeService recipeService, IUserService userService)
    {
        _recipeService = recipeService;
        _userService = userService;
    }

    [HttpGet(Name = "GetRecipes")]
    public async Task<IEnumerable<RecipeDto>> Get(string? title = null)
    {
        await Task.Delay(2000);
        string userId = await _userService.GetCurrentUserId();
        RecipeDto[] recipes = await _recipeService.Get(userId, title);
        return recipes;
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<RecipeDto?> GetById(int id)
    {
        await Task.Delay(2000);
        string userId = await _userService.GetCurrentUserId();
        RecipeDto? recipe = await _recipeService.GetById(userId, id);
        return recipe;
    }

    [HttpPost(Name = "CreateRecipe")]
    public async Task<int> Create([FromBody] RecipeCreateDto dto)
    {
        string userId = await _userService.GetCurrentUserId();
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
