using Bookery.Node.Common;
using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Common.DTOs.Output;
using Bookery.Node.Data;
using Bookery.Node.Data.Entities;
using Bookery.Node.Exceptions;
using Bookery.Node.Mappers;
using Bookery.Node.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Services.Implementations;

public class NodeService : INodeService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    private readonly PathBuilder _pathBuilder = new();


    private readonly IStorageProducer _storageProducer;

    public NodeService(IDbContextFactory<AppDbContext> contextFactory, IStorageProducer storageProducer)
    {
        _contextFactory = contextFactory;
        _storageProducer = storageProducer;
    }

    public async Task<List<NodeDto>> GetByPath(string? path, Guid userId)
    {
        var nodeResult = await GetPrivateNode(userId, path, false);

        var nodes = nodeResult.LevelTree.Children.Select(x => x.Data);

        return nodes.Select(NodeMapper.ToDto).ToList();
    }

    public async Task<(NodeDto Node, string Path)> Create(string? path, CreateNodeDto createNodeDto, Guid userId)
    {
        var nodeResult = await GetPrivateNode(userId, path, false);

        if (nodeResult.LevelTree.Children.All(x => x.Data.Name != createNodeDto.Name))
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            _pathBuilder.ParsePath(path);
            _pathBuilder.AddNode(createNodeDto.Name);

            var entity = new NodeEntity()
            {
                Id = Guid.NewGuid(),
                Name = createNodeDto.Name,
                IsDirectory = createNodeDto.IsDirectory,
                Size = null,
                ParentId = !nodeResult.LevelTree.IsRoot ? nodeResult.LevelTree.Data.Id : null,
                OwnerId = userId,
                CreatedAt = timestamp,
                CreatedById = userId,
                ModifiedAt = timestamp,
                ModifiedById = userId,
            };

            await using var context = await _contextFactory.CreateDbContextAsync();

            var createdResult = await context.Nodes.AddAsync(entity);
            await context.SaveChangesAsync();

            var dto = NodeMapper.ToDto(createdResult.Entity);

            return (dto, _pathBuilder.GetPath());
        }

        throw new NodeAlreadyExistsException(createNodeDto.Name, path);
    }

    public async Task<NodeEntity> Create(NodeEntity entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        entity.Id = new Guid();

        var createdResult = await context.Nodes.AddAsync(entity);
        await context.SaveChangesAsync();

        return createdResult.Entity;
    }

    public async Task<(NodeDto Node, string Path)> Update(string? path, UpdateNodeDto updateNodeDto, Guid userId)
    {
        var nodeResult = await GetPrivateNode(userId, path, true);

        if (nodeResult.LevelTree.Children.Where(x => x.Data.Id != nodeResult.Node?.Id)
            .All(x => x.Data.Name != updateNodeDto.Name))
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.Nodes
                .FirstOrDefaultAsync(x => x.Id == nodeResult.Node.Id);

            if (entity == null)
            {
                throw new NodeDoesNotExistException();
            }

            entity.Name = updateNodeDto.Name?.Length > 0 ? updateNodeDto.Name : entity.Name;
            entity.Size = (updateNodeDto.Size ?? 0) > 0 ? updateNodeDto.Size : entity.Size;
            entity.ModifiedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            entity.ModifiedById = updateNodeDto.ModifiedById == Guid.Empty ? userId : updateNodeDto.ModifiedById;

            var updatedEntity = context.Nodes.Update(entity);
            await context.SaveChangesAsync();

            _pathBuilder.ParsePath(path);
            _pathBuilder.GetLastNode();
            _pathBuilder.AddNode(updateNodeDto.Name);

            var dto = NodeMapper.ToDto(updatedEntity.Entity);

            return (dto, _pathBuilder.GetPath());
        }

        throw new NodeAlreadyExistsException(updateNodeDto.Name ?? string.Empty, path);
    }

    public async Task Delete(string? path, Guid userId)
    {
        var nodeResult = await GetPrivateNode(userId, path, true);

        var deletedChildren =
            DeleteChildren(nodeResult.LevelTree.Children.FirstOrDefault(x => x.Data.Name == nodeResult.Node.Name));

        await using var context = await _contextFactory.CreateDbContextAsync();

        var entities = await context.Nodes.Where(x => deletedChildren.NodeIdsToDelete.Contains(x.Id)).ToListAsync();

        context.Nodes.RemoveRange(entities);

        var entity = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeResult.Node.Id);

        if (entity != null)
        {
            context.Nodes.Remove(entity);
        }

        await context.SaveChangesAsync();

        foreach (var fileId in deletedChildren.FileIdsToDelete)
        {
            _storageProducer.Delete(fileId);
        }

        if (!nodeResult.Node.IsDirectory)
        {
            _storageProducer.Delete(nodeResult.Node.Id);
        }
    }

    private (List<Guid> NodeIdsToDelete, List<Guid> FileIdsToDelete) DeleteChildren(
        TreeExtensions.ITree<NodeEntity> node)
    {
        var nodeIdsToDelete = new List<Guid>();
        var fileIdsToDelete = new List<Guid>();
        foreach (var child in node.Children)
        {
            if (!child.IsLeaf)
            {
                var deletedChildren = DeleteChildren(child);
                nodeIdsToDelete.AddRange(deletedChildren.NodeIdsToDelete);
                fileIdsToDelete.AddRange(deletedChildren.FileIdsToDelete);
            }

            nodeIdsToDelete.Add(child.Data.Id);
            if (!child.Data.IsDirectory)
            {
                fileIdsToDelete.Add(child.Data.Id);
            }
        }

        return (nodeIdsToDelete, fileIdsToDelete);
    }

    private async Task<NodeResult> GetPrivateNode(Guid userId, string? path, bool isPreLevelTree)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var allNodes = await context.Nodes.Where(x => x.OwnerId == userId).ToListAsync();
        var virtualRoot = allNodes.ToTree((parent, child) => child.ParentId == parent.Id);

        _pathBuilder.ParsePath(path);

        if (isPreLevelTree)
        {
            _pathBuilder.GetLastNode();
        }

        var levelTree = TreeExtensions.GetLevelTree(virtualRoot, _pathBuilder.GetPath());

        if (levelTree == null)
        {
            throw new NodeDoesNotExistException();
        }

        if (!isPreLevelTree)
        {
            return new NodeResult(levelTree, null);
        }

        var node = levelTree.Children.FirstOrDefault(x => x.Data.Name == _pathBuilder.GetLastNode(path));

        if (node == null)
        {
            throw new NodeDoesNotExistException();
        }

        return new NodeResult(levelTree, node.Data);
    }

    private record NodeResult(TreeExtensions.ITree<NodeEntity> LevelTree, NodeEntity? Node);
}