using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Exceptions;
using Management.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Auth0;
using Shared.Auth0.Models;

namespace Management.Infrastructure.EF.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ManagementDbContext _dbContext;
    private readonly IAuth0Client _auth0Client;

    public UserRepository(IHttpContextAccessor httpContextAccessor, ManagementDbContext dbContext, IAuth0Client auth0Client)
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

    public async Task<UserDto> GetUser(string userId)
    {
        var entity = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId)
            ?? await GetUserFromAuth0AndSave(userId);

        return new UserDto
        {
            AvatarUrl = entity.Picture,
            DisplayName = entity.DisplayName,
            Id = entity.Id
        };
    }

    public async Task<List<UserDto>> GetUsers(IEnumerable<string> userIds)
    {
        List<User> entities = await _dbContext.Users.AsNoTracking().Where(x => userIds.Contains(x.Id)).ToListAsync();
        HashSet<string> entityIds = entities.Select(x => x.Id).ToHashSet();
        IEnumerable<string> missingIds = userIds.Where(id => !entityIds.Contains(id));
        IEnumerable<Task<User>> tasks = missingIds.Select(id => GetUserFromAuth0AndSave(id));
        User[] missingUsers = await Task.WhenAll(tasks);
        List<UserDto> dtos = entities.Union(missingUsers).Select(user => new UserDto
        {
            AvatarUrl = user.Picture,
            DisplayName = user.DisplayName,
            Id = user.Id
        }).ToList();

        return dtos;
    }

    private async Task EnsureUserExist(string userId)
    {
        bool userExist = await _dbContext.Users.AnyAsync(x => x.Id == userId);
        if (userExist)
        {
            return;
        }

        await GetUserFromAuth0AndSave(userId);
    }

    private async Task<User> GetUserFromAuth0AndSave(string userId)
    {
        UserResponse auth0User = await _auth0Client.GetUser(userId)
            ?? throw new InvalidOperationException($"Could not get a user from Auth0 with the given ID '{userId}'.");

        var user = new User
        {
            Id = userId,
            DisplayName = auth0User.Name ?? auth0User.Username ?? auth0User.Email ?? auth0User.UserId.Substring(auth0User.UserId.Length - 6),
            Picture = auth0User.Picture,
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}
