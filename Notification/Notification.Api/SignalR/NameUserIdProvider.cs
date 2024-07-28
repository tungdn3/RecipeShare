using Microsoft.AspNetCore.SignalR;

namespace Notification.Api.SignalR;

public class NameUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Identity?.Name;
    }
}
