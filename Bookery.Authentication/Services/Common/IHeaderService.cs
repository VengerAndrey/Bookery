using Bookery.Authentication.Models;

namespace Bookery.Authentication.Services.Common;

public interface IHeaderService
{
    Task<StsUser?> GetRequestUser(HttpRequest request);
}