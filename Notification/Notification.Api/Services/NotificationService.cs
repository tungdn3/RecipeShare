using Microsoft.EntityFrameworkCore;
using Notification.Api.Data;
using Notification.Api.Data.Entities;
using Notification.Api.Dtos;
using Notification.Api.Repositories;

namespace Notification.Api.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly UserRepository _userRepository;
    private readonly NotificationDbContext _dbContext;

    public NotificationService(
        ILogger<NotificationService> logger,
        UserRepository userRepository,
        NotificationDbContext dbContext)
    {
        _logger = logger;
        _userRepository = userRepository;
        _dbContext = dbContext;
    }

    public async Task<PageResultDto<NotificationMessageDto>> GetNotSeenNotifications(int pageNumber, int pageSize, int? lastId = null)
    {
        string userId = _userRepository.GetCurrentUserId();
        IQueryable<NotificationMessage> query = _dbContext.NotificationMessages
            .AsNoTracking()
            .Where(x => x.ToUserId == userId && !x.IsSeen);

        if (lastId.HasValue)
        {
            query = query.Where(x => x.Id > lastId.Value);
        }

        int totalCount = await query.CountAsync();
        List<NotificationMessageDto> items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(x => new NotificationMessageDto
            {
                CommentId = x.CommentId,
                CreatedAt = x.CreatedAt,
                Id = x.Id,
                IsSeen = x.IsSeen,
                ParentCommentId = x.ParentCommentId,
                RecipeId = x.RecipeId,
                SeenAt = x.SeenAt,
                Type = x.Type,
                FromUserDisplayName = x.FromUser.DisplayName,
                FromUserId = x.FromUserId,
                ToUserId = x.ToUserId,
            })
            .ToListAsync();

        return new PageResultDto<NotificationMessageDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task MarkAsSeen(List<long> notificationMessageIds)
    {
        string userId = _userRepository.GetCurrentUserId();
        List<NotificationMessage> notificationsToUpdate = await _dbContext.NotificationMessages
            .Where(x => x.ToUserId == userId && notificationMessageIds.Contains(x .Id))
            .ToListAsync();

        DateTime now = DateTime.UtcNow;
        foreach (NotificationMessage item in notificationsToUpdate)
        {
            item.IsSeen = true;
            item.SeenAt = now;
        }

        await _dbContext.SaveChangesAsync();
    }
}
