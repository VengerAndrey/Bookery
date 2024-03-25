using Bookery.Common.Results;
using Bookery.User.Extensions;
using Bookery.User.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.User.Controllers;

[Route("api/Photo")]
[ApiController]
public class PhotoController : ControllerBase
{
    private readonly ILogger<PhotoController> _logger;
    private readonly IPhotoService _photoService;

    public PhotoController(ILogger<PhotoController> logger, IPhotoService photoService)
    {
        _logger = logger;
        _photoService = photoService;
    }

    [HttpGet]
    [Route("Profile")]
    public async Task<IActionResult> DownloadProfilePhoto()
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            var stream = await _photoService.DownloadProfilePhoto(userId.Value);

            if (stream == null)
            {
                return new NotFoundResult();
            }

            return File(stream, "application/octet-stream");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(PhotoController)}.{nameof(DownloadProfilePhoto)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpPost]
    [Route("Profile")]
    public async Task<IActionResult> UploadProfilePhoto([FromForm] IFormFile file)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            var success = await _photoService.UploadProfilePhoto(userId.Value, file.OpenReadStream());

            return success ? new OkResult() : new StatusCodeResult(500);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(PhotoController)}.{nameof(UploadProfilePhoto)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}