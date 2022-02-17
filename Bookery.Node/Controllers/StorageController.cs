using Bookery.Node.Models;
using Bookery.Node.Services.Common;
using Bookery.Node.Services.Node;
using Bookery.Node.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[Route("api/Storage/{id}")]
[ApiController]
public class StorageController : ControllerBase
{
    private readonly IHeaderService _headerService;
    private readonly INodeService _nodeService;
    private readonly IStorageService _storageService;
    private readonly IUserNodeService _userNodeService;

    public StorageController(IStorageService storageService, INodeService nodeService, IUserNodeService userNodeService,
        IHeaderService headerService)
    {
        _storageService = storageService;
        _nodeService = nodeService;
        _userNodeService = userNodeService;
        _headerService = headerService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(Guid id, [FromForm] IFormFile file)
    {
        var user = await _headerService.GetRequestUser(Request);
        if (user is null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var node = await _nodeService.Get(id);
        if (node is null)
        {
            return NotFound();
        }

        var userNode = (await _userNodeService.GetAll())
            .Where(x => x.UserId == user.Id)
            .FirstOrDefault(x => x.NodeId == id);

        if (node.OwnerId == user.Id || userNode != null && userNode.AccessTypeId == AccessTypeId.Write)
        {
            node.Size = file.Length;
            node.ModificationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            node.ModifiedById = node.OwnerId == user.Id ? node.OwnerId : userNode.UserId;

            await _nodeService.Update(node);

            var result = await _storageService.Upload(id, file.OpenReadStream());

            if (result)
            {
                return Ok();
            }

            return Problem();
        }

        return StatusCode(StatusCodes.Status403Forbidden);
    }

    [HttpGet]
    public async Task<IActionResult> Download(Guid id)
    {
        var user = await _headerService.GetRequestUser(Request);
        if (user is null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var node = await _nodeService.Get(id);
        if (node is null)
        {
            return NotFound();
        }

        var userNode = (await _userNodeService.GetAll())
            .Where(x => x.UserId == user.Id)
            .FirstOrDefault(x => x.NodeId == id);

        if (node.OwnerId == user.Id || userNode != null)
        {
            var stream = await _storageService.Download(id);
            if (stream == Stream.Null)
            {
                return NotFound();
            }

            return File(stream, "application/octet-stream");
        }

        return StatusCode(StatusCodes.Status403Forbidden);
    }
}