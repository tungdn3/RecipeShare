using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Auth0;
using Social.Core.Dto;
using Social.Core.Entities;
using Social.Core.Exceptions;
using Social.Core.Interfaces;
using Social.Infrastructure.EF;

namespace Social.Infrastructure.Auth0;

internal class UserRepository : IUserRepository
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SocialDbContext _dbContext;
    private readonly IAuth0Client _auth0Client;

    public UserRepository(IHttpContextAccessor httpContextAccessor, SocialDbContext dbContext, IAuth0Client auth0Client)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _auth0Client = auth0Client;
    }

    public async Task<string> GetCurrentUserId()
    {
        if (_httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new NotAuthenticatedException();
        }

        string userId = _httpContextAccessor.HttpContext.User.Identity.Name!;
        await EnsureUserExist(userId);
        return userId;
    }

    public async Task<List<UserDto>> GetUsers(string[] userIds)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new UserDto
            {
                AvatarUrl = x.Picture,
                DisplayName = x.DisplayName,
                Id = x.Id
            }).ToListAsync();

        return users;
    }

    private async Task EnsureUserExist(string userId)
    {
        bool userExist = await _dbContext.Users.AnyAsync(x => x.Id == userId);
        if (userExist)
        {
            return;
        }

        var user = await _auth0Client.GetUser(userId)
            ?? throw new InvalidOperationException($"Could not find a user with the given ID '{userId}'.");

        _dbContext.Users.Add(new User
        {
            Id = userId,
            DisplayName = user.Name ?? user.Username ?? user.Email ?? user.UserId.Substring(user.UserId.Length - 6),
            Picture = user.Picture,
        });
        await _dbContext.SaveChangesAsync();
    }
}
