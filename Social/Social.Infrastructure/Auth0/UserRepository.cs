using Microsoft.AspNetCore.Http;
using Social.Core.Exceptions;
using Social.Core.Interfaces;

namespace Social.Infrastructure.Auth0;

internal class UserRepository : IUserRepository
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRepository(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUserId()
    {
        if (_httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new NotAuthenticatedException();
        }

        string identityName = _httpContextAccessor.HttpContext.User.Identity.Name!;
        return identityName;
    }
}
