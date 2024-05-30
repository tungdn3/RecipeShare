using Microsoft.EntityFrameworkCore;
using Social.Core.Dto;
using Social.Core.Entities;
using Social.Core.Interfaces;

namespace Social.Infrastructure.EF;

public class CommentRepository : ICommentRepository
{
    private readonly SocialDbContext _context;

    public CommentRepository(SocialDbContext context)
    {
        _context = context;
    }

    public async Task<int> Add(Comment comment)
    {
        _context.Add(comment);
        await _context.SaveChangesAsync();
        return comment.Id;
    }

    public Task<bool> Exists(int id)
    {
        return _context.Comments.AnyAsync(x => x.Id == id);
    }

    public Task<Comment?> GetById(int id)
    {
        return _context.Comments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PageResultDto<CommentDto>> GetReplies(int commentId, int pageNumber, int pageSize)
    {
        IQueryable<Comment> query = _context.Comments
            .AsNoTracking()
            .Where(x => x.ParentId == commentId && !x.IsDeleted);

        PageResultDto<CommentDto> result = await BuildPageResult(query, pageNumber, pageSize);
        return result;
    }

    public async Task<PageResultDto<CommentDto>> GetByRecipe(int recipeId, int pageNumber, int pageSize)
    {
        IQueryable<Comment> query = _context.Comments
            .AsNoTracking()
            .Where(x => x.RecipeId == recipeId && x.Path == string.Empty && !x.IsDeleted);

        PageResultDto<CommentDto> result = await BuildPageResult(query, pageNumber, pageSize);
        return result;
    }

    private async Task<PageResultDto<CommentDto>> BuildPageResult(IQueryable<Comment> query, int pageNumber, int pageSize)
    {
        int totalCount = await query.CountAsync();

        List<Comment> replies = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (replies.Count == 0)
        {
            return new PageResultDto<CommentDto>
            {
                Items = new List<CommentDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        List<int> replyIds = replies.Select(x => x.Id).ToList();
        var counts = await _context.Comments
            .Where(x => x.ParentId.HasValue && replyIds.Contains(x.ParentId.Value))
            .GroupBy(x => x.ParentId)
            .Select(g => new
            {
                ReplyId = g.Key,
                ReplyCount = g.Count()
            })
            .ToListAsync();

        var items = replies.Select(x => new CommentDto
        {
            Id = x.Id,
            ParentId = x.ParentId,
            UserId = x.UserId,
            Content = x.Content,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            ReplyCount = counts.Find(c => c.ReplyId == x.Id)?.ReplyCount ?? 0,
            Path = x.Path,
        }).ToList();

        return new PageResultDto<CommentDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    public async Task Delete(int id)
    {
        Comment? comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return;
        }

        comment.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task Save(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }
}
