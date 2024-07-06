// Ignore Spelling: API

using Management.Core.CategoryUseCases.Queries;
using Management.Core.Commands;
using Management.Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers;

[Authorize]
[ApiController]
[Route("management/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(Name = "GetCategories")]
    public async Task<IEnumerable<CategoryDto>> Get(string? name = null)
    {
        List<CategoryDto> categories = await _mediator.Send(new GetCategories.GetCategoriesRequest
        {
            Name = name,
        });

        return categories;
    }

    [HttpPost(Name = "CreateCategory")]
    public async Task<int> Create([FromBody] CreateCategory.CreateCategoryRequest request)
    {
        int id = await _mediator.Send(request);
        return id;
    }

    [HttpPut("{id}", Name = "UpdateCategory")]
    public async Task<IActionResult> Update(int id, UpdateCategory.UpdateCategoryRequest request)
    {
        request.Id = id;
        await _mediator.Send(request);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteCategory")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteCategory.DeleteCategoryRequest
        {
            Id = id,
        });

        return NoContent();
    }
}
