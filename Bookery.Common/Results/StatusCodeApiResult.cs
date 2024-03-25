using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Common.Results;

public class StatusCodeApiResult : StatusCodeResult
{
    public StatusCodeApiResult(HttpStatusCode statusCode) : base((int)statusCode)
    {
    }
}