using System.Collections.ObjectModel;

namespace Management.Core.Dtos;

public class RecipeCreateDto
{
    public int CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int PreparationMinutes { get; set; }

    public int CookingMinutes { get; set; }

    public Collection<string> Ingredients { get; set; } = [];

    public string Instructions { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
}
