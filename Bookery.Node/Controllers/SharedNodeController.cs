using Bookery.Node.Common;
using Bookery.Node.Models;
using Bookery.Node.Services.Common;
using Bookery.Node.Services.Node;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[ApiController]
[Route("api/Node/Shared/{*path}")]
public class SharedNodeController : ControllerBase
{
    private readonly IHeaderService _headerService;
    private readonly INodeService _nodeService;
    private readonly PathBuilder _pathBuilder;
    private readonly IUserNodeService _userNodeService;

    public SharedNodeController(INodeService nodeService, IUserService userService, IUserNodeService userNodeService,
        IHeaderService headerService)
    {
        _nodeService = nodeService;
        _userNodeService = userNodeService;
        _headerService = headerService;
        _pathBuilder = new PathBuilder();
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? path)
    {
        var user = await _headerService.GetRequestUser(Request);

        if (user is null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var allNodes = (await _userNodeService.GetAll())
            .Where(x => x.UserId == user.Id)
            .Select(async x => await _nodeService.Get(x.NodeId))
            .Select(x => x.Result)
            .ToList();

        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);

        var levelTree = TreeExtensions.GetLevelTree(virtualRoot, path, true);

        if (levelTree is null)
        {
            return NotFound();
        }

        var nodes = levelTree.Children.Select(x => x.Data).ToList();

        return new JsonResult(nodes);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string? path, [FromBody] Models.Node create)
    {
        var user = await _headerService.GetRequestUser(Request);
        var userNodeResult = await GetSharedNode(user, path, false, true);

        if (userNodeResult.ActionResult != null)
        {
            return userNodeResult.ActionResult;
        }

        if (userNodeResult.LevelTree.Children.All(x => x.Data.Name != create.Name))
        {
            if (userNodeResult.UserNode.AccessTypeId != AccessTypeId.Write)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            _pathBuilder.ParsePath(path);
            _pathBuilder.AddNode(create.Name);

            create.OwnerId = userNodeResult.LevelTree.Data.OwnerId;
            create.ModifiedById = user.Id;
            create.CreatedById = user.Id;

            if (!userNodeResult.LevelTree.IsRoot)
            {
                create.ParentId = userNodeResult.LevelTree.Data.Id;
            }

            create.CreationTimestamp = timestamp;
            create.ModificationTimestamp = timestamp;

            var created = await _nodeService.Create(create);

            ShareNode(new UserNode
            {
                UserId = user.Id,
                NodeId = created.Id,
                Timestamp = timestamp,
                AccessTypeId = AccessTypeId.Write
            });

            return Created(_pathBuilder.GetPath(), created);
        }

        return Conflict();
    }

    [HttpPut]
    public async Task<IActionResult> Update(string path, [FromBody] Models.Node update)
    {
        var user = await _headerService.GetRequestUser(Request);
        var userNodeResult = await GetSharedNode(user, path, true, true);

        if (userNodeResult.ActionResult != null)
        {
            return userNodeResult.ActionResult;
        }

        if (userNodeResult.UserNode.AccessTypeId != AccessTypeId.Write)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var entity = await _nodeService.Get(userNodeResult.Node.Id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = update.Name?.Length > 0 ? update.Name : entity.Name;
        if (update.Size == -1)
        {
            entity.Size = null;
        }
        else
        {
            entity.Size = (update.Size ?? 0) > 0 ? update.Size : entity.Size;
        }

        entity.ModificationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        entity.ModifiedById = user.Id;

        await _nodeService.Update(entity);

        _pathBuilder.ParsePath(path);
        _pathBuilder.GetLastNode();
        _pathBuilder.AddNode(update.Name);

        return Accepted(_pathBuilder.GetPath());
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

    private async Task<UserNodeResult> GetSharedNode(User? user, string? path, bool isPreLevelTree,
        bool differentRoots = false)
    {
        var userNodeResult = new UserNodeResult();

        if (user is null)
        {
            userNodeResult.ActionResult = StatusCode(StatusCodes.Status401Unauthorized);
            return userNodeResult;
        }

        var allUserNodes = (await _userNodeService.GetAll()).Where(x => x.UserId == user.Id).ToList();
        var allUserNodesIds = allUserNodes.Select(x => x.NodeId);
        var allNodes = (await _nodeService.GetAll()).Where(x => allUserNodesIds.Contains(x.Id)).ToList();
        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);

        _pathBuilder.ParsePath(path);

        if (isPreLevelTree)
        {
            _pathBuilder.GetLastNode();
        }

        var levelTree = TreeExtensions.GetLevelTree(virtualRoot, _pathBuilder.GetPath(), differentRoots);

        if (levelTree is null)
        {
            userNodeResult.ActionResult = NotFound();
            return userNodeResult;
        }

        userNodeResult.LevelTree = levelTree;

        if (!isPreLevelTree)
        {
            userNodeResult.UserNode = allUserNodes.FirstOrDefault(x => x.NodeId == levelTree.Data.Id);
            return userNodeResult;
        }

        var node = levelTree.Children.FirstOrDefault(x =>
            (differentRoots && _pathBuilder.GetPath() == string.Empty ? x.Data.Id.ToString() : x.Data.Name) ==
            _pathBuilder.GetLastNode(path));

        if (node is null)
        {
            userNodeResult.ActionResult = NotFound();
            return userNodeResult;
        }

        userNodeResult.Node = node.Data;
        userNodeResult.UserNode = allUserNodes.FirstOrDefault(x => x.NodeId == node.Data.Id);

        return userNodeResult;
    }

    private class UserNodeResult
    {
        public IActionResult? ActionResult { get; set; }
        public TreeExtensions.ITree<Models.Node> LevelTree { get; set; }
        public Models.Node Node { get; set; }
        public UserNode UserNode { get; set; }
    }
}