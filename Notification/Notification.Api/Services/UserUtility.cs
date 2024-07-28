using Notification.Api.Data;
using Notification.Api.Data.Entities;
using Shared.Auth0;

namespace Notification.Api.Services;

public class UserUtility
{
    private readonly NotificationDbContext _dbContext;
    private readonly IAuth0Client _auth0Client;

    public UserUtility(NotificationDbContext dbContext, IAuth0Client auth0Client)
    {
        _dbContext = dbContext;
        _auth0Client = auth0Client;
    }

    public async Task<User> EnsureUserExist(string userId)
    {
        User? existingUser = await _dbContext.Users.FindAsync(userId);
        if (existingUser is not null)
        {
            return existingUser;
        }

        Shared.Auth0.Models.UserResponse auth0User = await _auth0Client.GetUser(userId)
            ?? throw new InvalidOperationException($"Failed to get Auth0 user with the given ID '{userId}'.");

        var user = new User
        {
            Id = userId,
            DisplayName = auth0User.Name ?? auth0User.Username ?? auth0User.Email ?? auth0User.UserId.Substring(auth0User.UserId.Length - 6),
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();
        return user;
    }
}
