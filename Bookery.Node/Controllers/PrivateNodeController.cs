using Bookery.Node.Common;
using Bookery.Node.Models;
using Bookery.Node.Services.Common;
using Bookery.Node.Services.Node;
using Bookery.Node.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[ApiController]
[Route("api/Node/Private/{*path}")]
public class PrivateNodeController : ControllerBase
{
    private readonly IHeaderService _headerService;
    private readonly INodeService _nodeService;
    private readonly IStorageProducer _storageProducer;
    private readonly PathBuilder _pathBuilder;

    public PrivateNodeController(INodeService nodeService, IHeaderService headerService,
        IUserNodeService userNodeService, IStorageProducer storageProducer)
    {
        _nodeService = nodeService;
        _headerService = headerService;
        _storageProducer = storageProducer;
        _pathBuilder = new PathBuilder();
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? path)
    {
        var user = await _headerService.GetRequestUser(Request);

        var nodeResult = await GetPrivateNode(user, path, false);

        if (nodeResult.ActionResult != null)
        {
            return nodeResult.ActionResult;
        }

        var nodes = nodeResult.LevelTree.Children.Select(x => x.Data).ToList();

        return new JsonResult(nodes);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string? path, [FromBody] Models.Node create)
    {
        var user = await _headerService.GetRequestUser(Request);
        var nodeResult = await GetPrivateNode(user, path, false);

        if (nodeResult.ActionResult != null)
        {
            return nodeResult.ActionResult;
        }

        if (nodeResult.LevelTree.Children.All(x => x.Data.Name != create.Name))
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            _pathBuilder.ParsePath(path);
            _pathBuilder.AddNode(create.Name);

            create.OwnerId = user.Id;
            create.ModifiedById = user.Id;
            create.CreatedById = user.Id;

            if (!nodeResult.LevelTree.IsRoot)
            {
                create.ParentId = nodeResult.LevelTree.Data.Id;
            }

            create.CreationTimestamp = timestamp;
            create.ModificationTimestamp = timestamp;

            return Created(_pathBuilder.GetPath(), await _nodeService.Create(create));
        }

        return Conflict();
    }

    [HttpPut]
    public async Task<IActionResult> Update(string? path, [FromBody] Models.Node update)
    {
        var user = await _headerService.GetRequestUser(Request);
        var nodeResult = await GetPrivateNode(user, path, true);

        if (nodeResult.ActionResult != null)
        {
            return nodeResult.ActionResult;
        }

        if (nodeResult.LevelTree.Children.Where(x => x.Data.Id != nodeResult.Node.Id)
            .All(x => x.Data.Name != update.Name))
        {
            var entity = await _nodeService.Get(nodeResult.Node.Id);

            if (entity is null)
            {
                return NotFound();
            }

            entity.Name = update.Name?.Length > 0 ? update.Name : entity.Name;
            entity.Size = (update.Size ?? 0) > 0 ? update.Size : entity.Size;
            entity.ModificationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            entity.ModifiedById = update.ModifiedById == Guid.Empty ? user.Id : update.ModifiedById;

            await _nodeService.Update(entity);

            _pathBuilder.ParsePath(path);
            _pathBuilder.GetLastNode();
            _pathBuilder.AddNode(update.Name);

            return Accepted(_pathBuilder.GetPath());
        }

        return Conflict();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string? path)
    {
        var user = await _headerService.GetRequestUser(Request);
        var nodeResult = await GetPrivateNode(user, path, true);

        if (nodeResult.ActionResult != null)
        {
            return nodeResult.ActionResult;
        }

        await DeleteChildren(nodeResult.LevelTree.Children.FirstOrDefault(x => x.Data.Name == nodeResult.Node.Name));
        await _nodeService.Delete(nodeResult.Node.Id);

        if (!nodeResult.Node.IsDirectory)
        {
            _storageProducer.Delete(nodeResult.Node.Id);
        }

        return NoContent();
    }

    private async Task DeleteChildren(TreeExtensions.ITree<Models.Node> node)
    {
        foreach (var child in node.Children)
        {
            if (!child.IsLeaf)
            {
                await DeleteChildren(child);
            }

            await _nodeService.Delete(child.Data.Id);
            if (!child.Data.IsDirectory)
            {
                _storageProducer.Delete(child.Data.Id);
            }
        }
    }

    private async Task<NodeResult> GetPrivateNode(User? user, string? path, bool isPreLevelTree)
    {
        var nodeResult = new NodeResult();

        if (user is null)
        {
            nodeResult.ActionResult = Unauthorized();
            return nodeResult;
        }

        var allNodes = (await _nodeService.GetAll()).Where(x => x.OwnerId == user.Id).ToList();
        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);

        _pathBuilder.ParsePath(path);

        if (isPreLevelTree)
        {
            _pathBuilder.GetLastNode();
        }

        var levelTree = TreeExtensions.GetLevelTree(virtualRoot, _pathBuilder.GetPath());

        if (levelTree is null)
        {
            nodeResult.ActionResult = NotFound();
            return nodeResult;
        }

        nodeResult.LevelTree = levelTree;

        if (!isPreLevelTree)
        {
            return nodeResult;
        }

        var node = levelTree.Children.FirstOrDefault(x => x.Data.Name == _pathBuilder.GetLastNode(path));

        if (node is null)
        {
            nodeResult.ActionResult = NotFound();
            return nodeResult;
        }

        nodeResult.Node = node.Data;

        return nodeResult;
    }

    private class NodeResult
    {
        public IActionResult? ActionResult { get; set; }
        public TreeExtensions.ITree<Models.Node> LevelTree { get; set; }
        public Models.Node? Node { get; set; }
    }
}