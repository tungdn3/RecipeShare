using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.Api.Dtos;
using Notification.Api.Services;

namespace Notification.Api.Controllers;

[Authorize]
[ApiController]
[Route("notification/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly ILogger<NotificationsController> _logger;
    private readonly NotificationService _notificationService;

    public NotificationsController(ILogger<NotificationsController> logger, NotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    [HttpGet(Name = "GetNotSeen")]
    public async Task<PageResultDto<NotificationMessageDto>> GetNotSeen(int pageNumber = 1, int pageSize = 10, int? lastId = null)
    {
        PageResultDto<NotificationMessageDto> result = await _notificationService.GetNotSeenNotifications(pageNumber, pageSize, lastId);
        return result;
    }

    [HttpPost("mark-as-seen", Name = "MarkAsSeen")]
    public async Task MarkAsSeen([FromBody] List<long> notificationMessageIds)
    {
        await _notificationService.MarkAsSeen(notificationMessageIds);
    }
}
