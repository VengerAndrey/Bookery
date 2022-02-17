using Bookery.User.Services.Common;
using Bookery.User.Services.Photo;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.User.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhotoController : ControllerBase
{
    private readonly IHeaderService _headerService;
    private readonly IPhotoService _photoService;

    public PhotoController(IPhotoService photoService, IHeaderService headerService)
    {
        _photoService = photoService;
        _headerService = headerService;
    }

    [HttpGet]
    public async Task<IActionResult> DownloadProfilePhoto()
    {
        var user = await _headerService.GetRequestUser(Request);
        if (user is null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var stream = await _photoService.DownloadProfilePhoto(user.Id);
        if (stream == Stream.Null)
        {
            return NotFound();
        }

        return File(stream, "application/octet-stream");
    }

    [HttpPost]
    public async Task<IActionResult> UploadProfilePhoto([FromForm] IFormFile file)
    {
        var user = await _headerService.GetRequestUser(Request);
        if (user is null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var result = await _photoService.UploadProfilePhoto(user.Id, file.OpenReadStream());

        if (result)
        {
            return Ok();
        }

        return Problem();
    }
}