using Management.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Management.Core.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string> GetCurrentUserId()
    {
        if (_httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new NotAuthenticatedException();
        }

        string identityName = _httpContextAccessor.HttpContext.User.Identity.Name!;

        return Task.FromResult(identityName);
    }
}
