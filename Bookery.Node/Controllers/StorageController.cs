using Bookery.Common.Results;
using Bookery.Node.Exceptions;
using Bookery.Node.Extensions;
using Bookery.Node.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[Route("api/Storage")]
[ApiController]
public class StorageController : ControllerBase
{
    private readonly ILogger<StorageController> _logger;
    private readonly IStorageService _storageService;

    public StorageController(ILogger<StorageController> logger, IStorageService storageService)
    {
        _logger = logger;
        _storageService = storageService;
    }

    [HttpPost]
    [Route("{id}")]
    public async Task<IActionResult> Upload(Guid id, [FromForm] IFormFile file)
    {
        try
        {
            var userId = Request.GetRequiredUserId();

            var result = await _storageService.Upload(id, userId, file);

            return result ? new OkResult() : new StatusCodeResult(500);

        }
        catch (UnauthorizedActionException)
        {
            return new UnauthorizedResult();
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (ForbiddenActionException)
        {
            return new ForbiddenApiResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(StorageController)}.{nameof(Upload)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Download(Guid id)
    {
        try
        {
            var userId = Request.GetRequiredUserId();

            var stream = await _storageService.Download(id, userId);

            if (stream == null)
            {
                return new NotFoundResult();
            }
            
            return File(stream, "application/octet-stream");
        }
        catch (UnauthorizedActionException)
        {
            return new UnauthorizedResult();
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (ForbiddenActionException)
        {
            return new ForbiddenApiResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(StorageController)}.{nameof(Download)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}