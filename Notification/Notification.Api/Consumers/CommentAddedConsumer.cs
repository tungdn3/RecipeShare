using MassTransit;
using Notification.Api.Services;
using Shared.IntegrationEvents;

namespace Notification.Api.Consumers;

public class CommentAddedConsumer : IConsumer<CommentAdded>
{
    private readonly ILogger _logger;
    private readonly NotificationService _notificationService;

    public CommentAddedConsumer(ILogger<CommentAddedConsumer> logger, NotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public Task Consume(ConsumeContext<CommentAdded> context)
    {
        return _notificationService.AddNotification(context.Message);
    }
}
