namespace Search.API.Models;

public class SearchResultItemDto
{
    public int Id { get; set; }

    public int? CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int PreparationMinutes { get; set; }

    public int CookingMinutes { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}
