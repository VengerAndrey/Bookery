using System.Net;

namespace Bookery.Common.Results;

public class ForbiddenApiResult : StatusCodeApiResult
{
    public ForbiddenApiResult() : base(HttpStatusCode.Forbidden)
    {
    }
}