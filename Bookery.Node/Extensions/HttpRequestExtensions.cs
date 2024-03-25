using Bookery.Node.Exceptions;

namespace Bookery.Node.Extensions;

public static class HttpRequestExtensions
{
    public static Guid? GetUserId(this HttpRequest request)
    {
        if (request.Headers.TryGetValue("UserId", out var userId))
        {
            if (Guid.TryParse(userId, out var typedUserId))
            {
                return typedUserId;
            }
        }

        return null;
    }
    
    public static Guid GetRequiredUserId(this HttpRequest request)
    {
        if (request.Headers.TryGetValue("UserId", out var userId))
        {
            if (Guid.TryParse(userId, out var typedUserId))
            {
                return typedUserId;
            }
        }

        throw new UnauthorizedActionException();
    }
}