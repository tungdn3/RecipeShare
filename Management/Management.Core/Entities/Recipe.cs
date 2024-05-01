using System.Collections.ObjectModel;

namespace Management.Core.Entities;

public class Recipe
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? ImageFileName { get; set; }

    public int PreparationMinutes { get; set; }

    public int CookingMinutes { get; set; }

    public Collection<string> Ingredients { get; set; } = [];

    public string Instructions { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public bool IsPublished { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}
