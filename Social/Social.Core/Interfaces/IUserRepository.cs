using Social.Core.Dto;

namespace Social.Core.Interfaces;

public interface IUserRepository
{
    Task<string> GetCurrentUserId();

    Task<List<UserDto>> GetUsers(string[] userIds);
}
