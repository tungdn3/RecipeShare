namespace Search.API.Models;

public class RecipeSearchRequest
{
    public string? Query { get; set; }

    public int? CategoryId { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
