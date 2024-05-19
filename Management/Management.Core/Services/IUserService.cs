namespace Management.Core.Services;

public interface IUserService
{
    Task<string> GetCurrentUserId();
}
