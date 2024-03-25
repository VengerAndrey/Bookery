using Bookery.Common.Results;
using Bookery.Storage.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Storage.Controllers;

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
            var result = await _storageService.Upload(id, file.OpenReadStream());

            if (result)
            {
                return new OkResult();
            }

            return new InternalServerErrorApiResult();
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
            var stream = await _storageService.Download(id);

            if (stream == null)
            {
                return new NotFoundResult();
            }

            return File(stream, "application/octet-stream");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(StorageController)}.{nameof(Download)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}