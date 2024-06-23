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
    public async Task<PageResultDto<SearchResultItemDto>> SearchRecipes(int? categoryId = null, string? query = null, int pageNumber = 1, int pageSize = 10)
    {
        PageResultDto<SearchResultItemDto> pageResult = await _searchService.Search(new RecipeSearchRequest
        {
            CategoryId = categoryId,
            Query = query,
            PageNumber = pageNumber,
            PageSize = pageSize,
        });

        return pageResult;
    }

    [HttpGet("new", Name = "GetNewRecipes")]
    public async Task<PageResultDto<SearchResultItemDto>> GetNewRecipes(int pageNumber = 1, int pageSize = 10)
    {
        PageResultDto<SearchResultItemDto> pageResult = await _searchService.GetNewRecipes(pageNumber, pageSize);
        return pageResult;
    }

    [HttpGet("complete", Name = "Complete")]
    public async Task<IActionResult> Complete(string query, int top = 10)
    {
        List<string> titles = await _searchService.Complete(query, top);
        return Ok(titles);
    }
}
