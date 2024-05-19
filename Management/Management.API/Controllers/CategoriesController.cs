// Ignore Spelling: API

using Management.Core.Dtos;
using Management.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers;

[Authorize]
[ApiController]
[Route("management/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoryService _categoryService;

    public CategoriesController(ILogger<CategoriesController> logger, ICategoryService categoryService)
    {
        _logger = logger;
        _categoryService = categoryService;
    }

    [HttpGet(Name = "GetCategories")]
    public async Task<IEnumerable<CategoryDto>> Get(string? name = null)
    {
        IEnumerable<CategoryDto> categories = await _categoryService.Get(name);
        return categories;
    }

    [HttpPost(Name = "CreateCategory")]
    public async Task<int> Create([FromBody]CategoryCreateDto dto)
    {
        int id = await _categoryService.Create(dto);
        return id;
    }

    [HttpPut("{id}", Name = "UpdateCategory")]
    public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
    {
        await _categoryService.Update(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteCategory")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.Delete(id);
        return NoContent();
    }
}
