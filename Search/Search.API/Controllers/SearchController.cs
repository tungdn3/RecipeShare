using Microsoft.AspNetCore.Mvc;
using Search.API.Models;
using Search.API.Services;

namespace Search.API.Controllers;

[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;
    private readonly SearchService _searchService;

    public SearchController(ILogger<SearchController> logger, SearchService searchService)
    {
        _logger = logger;
        _searchService = searchService;
    }

    [HttpGet(Name = "SearchRecipes")]
    public async Task< IActionResult> SearchRecipes(int? categoryId = null, string? query = null, int page = 1, int pageSize = 10)
    {
        IReadOnlyCollection<Recipe> items = await _searchService.Search(new RecipeSearchRequest
        {
            CategoryId = categoryId,
            Query = query,
            Page = page,
            PageSize = pageSize,
        });

        return Ok(items);
    }

    [HttpGet("complete", Name = "Complete")]
    public async Task<IActionResult> Complete(string query, int top = 10)
    {
        List<string> titles = await _searchService.Complete(query, top);
        return Ok(titles);
    }
}
