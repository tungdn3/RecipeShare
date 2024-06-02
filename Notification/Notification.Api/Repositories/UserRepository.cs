using Notification.Api.Exceptions;

namespace Notification.Api.Repositories;

public class UserRepository
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRepository(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUserId()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true)
        {
            throw new NotAuthenticatedException();
        }

        string identityName = _httpContextAccessor.HttpContext.User.Identity.Name!;
        return identityName;
    }
}
