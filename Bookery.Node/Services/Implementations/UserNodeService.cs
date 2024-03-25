using Bookery.Node.Common;
using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Common.DTOs.Output;
using Bookery.Node.Common.Enums;
using Bookery.Node.Data;
using Bookery.Node.Data.Entities;
using Bookery.Node.Exceptions;
using Bookery.Node.Mappers;
using Bookery.Node.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Services.Implementations;

public class UserNodeService : IUserNodeService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    private readonly PathBuilder _pathBuilder = new();

    public UserNodeService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<NodeDto>> Get(string? path, Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var allNodes = await context.UserNodes
            .Include(x => x.Node)
            .Where(x => x.UserId == userId)
            .Select(x => x.Node)
            .ToListAsync();

        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);

        var levelTree = TreeExtensions.GetLevelTree(virtualRoot, path, true);

        if (levelTree == null)
        {
            throw new NodeDoesNotExistException();
        }

        var nodes = levelTree.Children.Select(x => x.Data);

        return nodes.Select(NodeMapper.ToDto).ToList();
    }

    public async Task<(NodeDto Node, string Path)> Create(string? path, CreateNodeDto createNodeDto, Guid userId)
    {
        var userNodeResult = await GetSharedNode(userId, path, false, true);

        if (userNodeResult.LevelTree.Children.All(x => x.Data.Name != createNodeDto.Name))
        {
            if (userNodeResult.UserNodeEntity.AccessTypeId != AccessTypeId.Write)
            {
                throw new InsufficientAccessLevelException();
            }

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            _pathBuilder.ParsePath(path);
            _pathBuilder.AddNode(createNodeDto.Name);

            var entity = new NodeEntity()
            {
                Id = Guid.NewGuid(),
                Name = createNodeDto.Name,
                IsDirectory = createNodeDto.IsDirectory,
                Size = null,
                ParentId = !userNodeResult.LevelTree.IsRoot ? userNodeResult.LevelTree.Data.Id : null,
                OwnerId = userNodeResult.LevelTree.Data.OwnerId,
                CreatedAt = timestamp,
                CreatedById = userId,
                ModifiedAt = timestamp,
                ModifiedById = userId,
            };

            await using var context = await _contextFactory.CreateDbContextAsync();

            var createdResult = await context.Nodes.AddAsync(entity);
            await context.SaveChangesAsync();

            ShareNode(new UserNodeEntity
            {
                UserId = userId,
                NodeId = entity.Id,
                SharedAt = timestamp,
                AccessTypeId = AccessTypeId.Write
            });

            var dto = NodeMapper.ToDto(createdResult.Entity);

            return (dto, _pathBuilder.GetPath());
        }

        throw new NodeAlreadyExistsException(createNodeDto.Name, path);
    }

    public async Task<(NodeDto Node, string Path)> Update(string? path, UpdateNodeDto updateNodeDto, Guid userId)
    {
        var userNodeResult = await GetSharedNode(userId, path, true, true);

        if (userNodeResult.UserNodeEntity.AccessTypeId != AccessTypeId.Write)
        {
            throw new InsufficientAccessLevelException();
        }

        await using var context = await _contextFactory.CreateDbContextAsync();

        var entity = await context.Nodes.FirstOrDefaultAsync(x => x.Id == userNodeResult.NodeEntity.Id);

        if (entity == null)
        {
            throw new NodeDoesNotExistException();
        }

        entity.Name = updateNodeDto.Name?.Length > 0 ? updateNodeDto.Name : entity.Name;
        if (updateNodeDto.Size == -1)
        {
            entity.Size = null;
        }
        else
        {
            entity.Size = (updateNodeDto.Size ?? 0) > 0 ? updateNodeDto.Size : entity.Size;
        }

        entity.ModifiedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        entity.ModifiedById = userId;

        var updatedResult = context.Nodes.Update(entity);
        await context.SaveChangesAsync();

        var dto = NodeMapper.ToDto(updatedResult.Entity);

        _pathBuilder.ParsePath(path);
        _pathBuilder.GetLastNode();
        _pathBuilder.AddNode(updateNodeDto.Name);

        return (dto, _pathBuilder.GetPath());
    }

    public async Task Share(ShareNodeDto shareNodeDto, Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == shareNodeDto.NodeId);
        if (node == null)
        {
            throw new NodeDoesNotExistException();
        }

        if (shareNodeDto.UserId == userId)
        {
            throw new InvalidActionException();
        }

        if (node.OwnerId != userId)
        {
            throw new ForbiddenActionException();
        }

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == shareNodeDto.UserId);
        if (user == null)
        {
            throw new UserDoesNotExistException();
        }

        var entity = new UserNodeEntity()
        {
            NodeId = shareNodeDto.NodeId,
            UserId = shareNodeDto.UserId,
            AccessTypeId = shareNodeDto.AccessTypeId,
            SharedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        ShareNode(entity);
    }

    public async Task Hide(HideNodeDto hideNodeDto, Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == hideNodeDto.NodeId);
        if (node == null)
        {
            throw new NodeDoesNotExistException();
        }

        if (node.OwnerId != userId)
        {
            throw new ForbiddenActionException();
        }

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == hideNodeDto.UserId);
        if (user == null)
        {
            throw new UserDoesNotExistException();
        }

        await HideNode(hideNodeDto);
    }

    public async Task<List<UserDto>> GetSharedWith(Guid nodeId, Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node == null)
        {
            throw new NodeDoesNotExistException();
        }

        if (node.OwnerId != userId)
        {
            throw new ForbiddenActionException();
        }

        var userEntities = (await context.UserNodes
                .Include(x => x.User)
                .Where(x => x.NodeId == node.Id)
                .Select(x => x.User)
                .ToListAsync())
            .OfType<UserEntity>();

        return userEntities.Select(UserMapper.ToDto).ToList();
    }

    public async Task<NodeDto> GetDetails(Guid nodeId, Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var userNode = context.UserNodes.FirstOrDefault(x => x.UserId == userId && x.NodeId == nodeId);

        if (userNode == null)
        {
            throw new NodeDoesNotExistException();
        }

        var node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);

        if (node == null)
        {
            throw new NodeDoesNotExistException();
        }

        return NodeMapper.ToDto(node);
    }

    private async Task<UserNodeEntity?> Create(UserNodeEntity entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var createdResult = await context.UserNodes.AddAsync(entity);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return null;
        }

        return createdResult.Entity;
    }

    private async Task<UserNodeEntity?> Update(UserNodeEntity entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.UserNodes.Update(entity);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return null;
        }

        return entity;
    }

    private async Task<bool> Delete(UserNodeEntity entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.UserNodes.Remove(entity);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async void ShareNode(UserNodeEntity userNodeEntity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var allNodes = await context.Nodes.ToListAsync();
        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);
        var node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == userNodeEntity.NodeId);

        var root = TreeExtensions.Find(virtualRoot, node);

        if (root != null)
        {
            await ShareChildren(root, userNodeEntity);

            var rootUserNode = new UserNodeEntity
            {
                NodeId = root.Data.Id,
                UserId = userNodeEntity.UserId,
                AccessTypeId = userNodeEntity.AccessTypeId,
                SharedAt = userNodeEntity.SharedAt
            };

            if (await Create(rootUserNode) is null)
            {
                await Update(rootUserNode);
            }
        }
    }

    private async Task ShareChildren(TreeExtensions.ITree<NodeEntity> node, UserNodeEntity userNodeEntity)
    {
        foreach (var child in node.Children)
        {
            if (!child.IsLeaf)
            {
                await ShareChildren(child, userNodeEntity);
            }

            var leafUserNode = new UserNodeEntity
            {
                NodeId = child.Data.Id,
                UserId = userNodeEntity.UserId,
                AccessTypeId = userNodeEntity.AccessTypeId,
                SharedAt = userNodeEntity.SharedAt
            };

            if (await Create(leafUserNode) == null)
            {
                await Update(leafUserNode);
            }
        }
    }

    private async Task HideNode(HideNodeDto userNodeEntity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var allNodes = await context.Nodes.ToListAsync();
        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);
        var node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == userNodeEntity.NodeId);

        var root = TreeExtensions.Find(virtualRoot, node);

        if (root != null)
        {
            await HideChildren(root, userNodeEntity);

            await Delete(new UserNodeEntity
            {
                NodeId = root.Data.Id,
                UserId = userNodeEntity.UserId
            });
        }
    }

    private async Task HideChildren(TreeExtensions.ITree<NodeEntity> node, HideNodeDto userNodeEntity)
    {
        foreach (var child in node.Children)
        {
            if (!child.IsLeaf)
            {
                await HideChildren(child, userNodeEntity);
            }

            await Delete(new UserNodeEntity
            {
                NodeId = child.Data.Id,
                UserId = userNodeEntity.UserId
            });
        }
    }

    private async Task<UserNodeResult> GetSharedNode(Guid userId, string? path, bool isPreLevelTree,
        bool differentRoots = false)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var allNodes = await context.UserNodes
            .Include(x => x.Node)
            .Where(x => x.UserId == userId)
            .Select(x => x.Node)
            .ToListAsync();

        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);

        _pathBuilder.ParsePath(path);

        if (isPreLevelTree)
        {
            _pathBuilder.GetLastNode();
        }

        var levelTree = TreeExtensions.GetLevelTree(virtualRoot, _pathBuilder.GetPath(), differentRoots);

        if (levelTree is null)
        {
            throw new NodeDoesNotExistException();
        }

        if (!isPreLevelTree)
        {
            var userNodeEntity1 = await context.UserNodes.FirstOrDefaultAsync(x => x.NodeId == levelTree.Data.Id);
            if (userNodeEntity1 == null)
            {
                throw new NodeDoesNotExistException();
            }

            return new UserNodeResult(levelTree, userNodeEntity1, null);
        }

        var node = levelTree.Children.FirstOrDefault(x =>
            (differentRoots && _pathBuilder.GetPath() == string.Empty ? x.Data.Id.ToString() : x.Data.Name) ==
            _pathBuilder.GetLastNode(path));

        if (node is null)
        {
            throw new NodeDoesNotExistException();
        }

        var userNodeEntity = await context.UserNodes.FirstOrDefaultAsync(x => x.NodeId == node.Data.Id);
        if (userNodeEntity == null)
        {
            throw new NodeDoesNotExistException();
        }

        return new UserNodeResult(levelTree, userNodeEntity, node.Data);
    }

    private record UserNodeResult(
        TreeExtensions.ITree<NodeEntity> LevelTree,
        UserNodeEntity UserNodeEntity,
        NodeEntity? NodeEntity
    );
}