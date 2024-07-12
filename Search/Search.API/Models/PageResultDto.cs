namespace Search.API.Models;

public class PageResultDto<T> where T : class
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public long TotalCount { get; set; }

    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    public bool HasPreviousPage => PageNumber > 0;

    public bool HasNextPage => PageNumber < TotalPages;
}
