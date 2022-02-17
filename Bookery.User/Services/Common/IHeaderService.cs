namespace Bookery.User.Services.Common;

public interface IHeaderService
{
    Task<Models.User?> GetRequestUser(HttpRequest request);
}