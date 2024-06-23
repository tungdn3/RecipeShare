using Management.Core.Dtos;

namespace Management.Core.Interfaces;

public interface IUserRepository
{
    Task<string> GetCurrentUserId();

    Task<UserDto> GetUser(string userId);

    Task<List<UserDto>> GetUsers(IEnumerable<string> userIds);
}
