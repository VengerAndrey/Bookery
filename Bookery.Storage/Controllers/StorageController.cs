using Bookery.Storage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Storage.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StorageController : ControllerBase
{
    private readonly IStorageService _storageService;

    public StorageController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    [Route("{id}")]
    public async Task<IActionResult> Upload(Guid id, [FromForm] IFormFile file)
    {
        var result = await _storageService.Upload(id, file.OpenReadStream());

        if (result)
        {
            return Ok();
        }

        return Problem();
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult Download(Guid id)
    {
        var stream = _storageService.Download(id);

        if (stream == Stream.Null)
        {
            return NotFound();
        }

        return File(stream, "application/octet-stream");
    }
}