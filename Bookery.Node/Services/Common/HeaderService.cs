using Bookery.Node.Models;
using Bookery.Node.Services.Node;

namespace Bookery.Node.Services.Common;

public class HeaderService : IHeaderService
{
    private readonly IUserService _userService;

    public HeaderService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<User?> GetRequestUser(HttpRequest request)
    {
        User? user = null;
        if (request.Headers.TryGetValue("UserId", out var userId))
        {
            user = await _userService.Get(Guid.Parse(userId));
        }

        return user;
    }
}