using System.Net;

namespace Bookery.Common.Results;

public class InternalServerErrorApiResult : StatusCodeApiResult
{
    public InternalServerErrorApiResult() : base(HttpStatusCode.InternalServerError)
    {
    }
}