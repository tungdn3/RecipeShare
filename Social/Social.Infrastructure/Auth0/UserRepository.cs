using Microsoft.AspNetCore.Http;
using Social.Core.Dto;
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

    public List<UserDto> GetUsers(string[] userIds)
    {
        return userIds.Select(id => new UserDto
        {
            AvatarUrl = "https://cdn.quasar.dev/img/avatar.png",
            DisplayName = $"user {id.Substring(id.Length - 3)}",
            Id = id
        }).ToList();
    }
}
