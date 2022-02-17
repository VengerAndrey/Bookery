using Bookery.Node.Models;

namespace Bookery.Node.Services.Common;

public interface IHeaderService
{
    Task<User?> GetRequestUser(HttpRequest request);
}