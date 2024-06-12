using MassTransit;
using Notification.Api.Services;
using Shared.IntegrationEvents;

namespace Notification.Api.Consumers;

public class LikeAddedConsumer : IConsumer<LikeAdded>
{
    private readonly ILogger<LikeAddedConsumer> _logger;
    private readonly NotificationService _notificationService;

    public LikeAddedConsumer(ILogger<LikeAddedConsumer> logger, NotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public Task Consume(ConsumeContext<LikeAdded> context)
    {
        return _notificationService.AddNotification(context.Message);
    }
}
