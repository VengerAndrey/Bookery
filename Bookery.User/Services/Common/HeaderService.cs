using Bookery.User.Services.User;

namespace Bookery.User.Services.Common;

public class HeaderService : IHeaderService
{
    private readonly IUserService _userService;

    public HeaderService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Models.User?> GetRequestUser(HttpRequest request)
    {
        Models.User? user = null;
        if (request.Headers.TryGetValue("UserId", out var userId))
        {
            user = await _userService.Get(Guid.Parse(userId));
        }

        return user;
    }
}