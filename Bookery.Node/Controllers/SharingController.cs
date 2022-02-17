using Bookery.Node.Common;
using Bookery.Node.Models;
using Bookery.Node.Services.Common;
using Bookery.Node.Services.Node;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[Route("api/Node/Sharing")]
[ApiController]
public class SharingController : ControllerBase
{
    private readonly IHeaderService _headerService;
    private readonly INodeService _nodeService;
    private readonly IUserNodeService _userNodeService;
    private readonly IUserService _userService;

    public SharingController(INodeService nodeService, IUserService userService, IUserNodeService userNodeService,
        IHeaderService headerService)
    {
        _nodeService = nodeService;
        _userService = userService;
        _userNodeService = userNodeService;
        _headerService = headerService;
    }

    [HttpPost]
    [Route("share")]
    public async Task<IActionResult> Share([FromBody] UserNode userNode)
    {
        var owner = await _headerService.GetRequestUser(Request);
        if (owner is null)
        {
            return Unauthorized();
        }

        var node = await _nodeService.Get(userNode.NodeId);
        if (node is null)
        {
            return NotFound();
        }

        if (userNode.UserId == owner.Id)
        {
            return BadRequest();
        }

        if (node.OwnerId != owner.Id)
        {
            return Forbid();
        }

        var user = await _userService.Get(userNode.UserId);
        if (user is null)
        {
            return NotFound();
        }

        userNode.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        ShareNode(userNode);

        return Accepted();
    }

    [HttpPost]
    [Route("hide")]
    public async Task<IActionResult> Hide([FromBody] UserNode userNode)
    {
        var owner = await _headerService.GetRequestUser(Request);
        if (owner is null)
        {
            return Unauthorized();
        }

        var node = await _nodeService.Get(userNode.NodeId);
        if (node is null)
        {
            return NotFound();
        }

        if (node.OwnerId != owner.Id)
        {
            return Forbid();
        }

        var user = await _userService.Get(userNode.UserId);
        if (user is null)
        {
            return NotFound();
        }

        HideNode(userNode);

        return NoContent();
    }

    [HttpGet]
    [Route("shared-with/{id}")]
    public async Task<IActionResult> GetSharing(Guid id)
    {
        var user = await _headerService.GetRequestUser(Request);
        if (user is null)
        {
            return Unauthorized();
        }

        var node = await _nodeService.Get(id);
        if (node is null)
        {
            return NotFound();
        }

        if (node.OwnerId != user.Id)
        {
            return Forbid();
        }

        var userNodes = (await _userNodeService.GetAll()).Where(x => x.NodeId == node.Id).ToList();

        return new JsonResult(userNodes);
    }

    [HttpGet]
    [Route("details/{id}")]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        var user = await _headerService.GetRequestUser(Request);
        if (user == null)
        {
            return Unauthorized();
        }

        var userNode = (await _userNodeService.GetAll())
            .FirstOrDefault(x => x.UserId == user.Id && x.NodeId == id);

        if (userNode is null)
        {
            return NotFound();
        }

        var node = await _nodeService.Get(id);

        if (node is null)
        {
            return NotFound();
        }

        return new JsonResult(node);
    }

    private async void ShareNode(UserNode userNode)
    {
        var allNodes = (await _nodeService.GetAll()).ToList();
        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);
        var node = await _nodeService.Get(userNode.NodeId);

        var root = TreeExtensions.Find(virtualRoot, node);

        if (root != null)
        {
            await ShareChildren(root, userNode);

            var rootUserNode = new UserNode
            {
                NodeId = root.Data.Id,
                UserId = userNode.UserId,
                AccessTypeId = userNode.AccessTypeId,
                Timestamp = userNode.Timestamp
            };

            if (await _userNodeService.Create(rootUserNode) is null)
            {
                await _userNodeService.Update(rootUserNode);
            }
        }
    }

    private async Task ShareChildren(TreeExtensions.ITree<Models.Node> node, UserNode userNode)
    {
        foreach (var child in node.Children)
        {
            if (!child.IsLeaf)
            {
                await ShareChildren(child, userNode);
            }

            var leafUserNode = new UserNode
            {
                NodeId = child.Data.Id,
                UserId = userNode.UserId,
                AccessTypeId = userNode.AccessTypeId,
                Timestamp = userNode.Timestamp
            };

            if (await _userNodeService.Create(leafUserNode) is null)
            {
                await _userNodeService.Update(leafUserNode);
            }
        }
    }

    private async void HideNode(UserNode userNode)
    {
        var allNodes = (await _nodeService.GetAll()).ToList();
        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);
        var node = await _nodeService.Get(userNode.NodeId);

        var root = TreeExtensions.Find(virtualRoot, node);

        if (root != null)
        {
            await HideChildren(root, userNode);

            await _userNodeService.Delete(new UserNode
            {
                NodeId = root.Data.Id,
                UserId = userNode.UserId
            });
        }
    }

    private async Task HideChildren(TreeExtensions.ITree<Models.Node> node, UserNode userNode)
    {
        foreach (var child in node.Children)
        {
            if (!child.IsLeaf)
            {
                await HideChildren(child, userNode);
            }

            await _userNodeService.Delete(new UserNode
            {
                NodeId = child.Data.Id,
                UserId = userNode.UserId
            });
        }
    }
}