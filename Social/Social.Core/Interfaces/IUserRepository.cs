using Social.Core.Dto;

namespace Social.Core.Interfaces;

public interface IUserRepository
{
    string GetCurrentUserId();
    List<UserDto> GetUsers(string[] userIds);
}
