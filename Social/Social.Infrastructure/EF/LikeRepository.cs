using Microsoft.EntityFrameworkCore;
using Social.Core.Dto;
using Social.Core.Entities;
using Social.Core.Interfaces;

namespace Social.Infrastructure.EF;

internal class LikeRepository : ILikeRepository
{
    private readonly SocialDbContext _context;

    public LikeRepository(SocialDbContext context)
    {
        _context = context;
    }

    public async Task<int> Add(Like like)
    {
        _context.Add(like);
        await _context.SaveChangesAsync();
        return like.Id;
    }

    public async Task<IEnumerable<CountDto>> CountRecipeLikes(IEnumerable<int> recipeIds)
    {
        List<CountDto> counts = await _context.Likes
            .Where(x => recipeIds.Contains(x.RecipeId))
            .GroupBy(x => x.RecipeId)
            .Select(g => new CountDto
            {
                Id = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        return counts;
    }

    public async Task<Like?> GetById(int id)
    {
        var like = await _context.Likes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return like;
    }

    public async Task Remove(int id)
    {
        var like = await _context.Likes.FirstOrDefaultAsync(x => x.Id == id);
        if (like != null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Like>> Search(int? recipeId = null, string? userId = null, int top = 10)
    {
        var query = _context.Likes.AsNoTracking();

        if (recipeId.HasValue)
        {
            query = query.Where(x => x.RecipeId == recipeId.Value);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(x => x.UserId == userId);
        }

        List<Like> items = await query
            .OrderByDescending(x => x.Id)
            .Take(top)
            .ToListAsync();

        return items;
    }
}
