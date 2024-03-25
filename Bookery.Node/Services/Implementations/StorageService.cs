using Bookery.Node.Common.Enums;
using Bookery.Node.Data;
using Bookery.Node.Exceptions;
using Bookery.Node.Services.Interfaces;
using Bookery.Storage.Common.Client;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Services.Implementations;

public class StorageService : IStorageService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly IStorageApiClient _storageApiClient;

    public StorageService(IDbContextFactory<AppDbContext> contextFactory, IStorageApiClient storageApiClient)
    {
        _contextFactory = contextFactory;
        _storageApiClient = storageApiClient;
    }

    public async Task<bool> Upload(Guid nodeId, Guid userId, IFormFile file)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node == null)
        {
            throw new NodeDoesNotExistException();
        }

        var userNode = await context.UserNodes.FirstOrDefaultAsync(x => x.NodeId == nodeId && x.UserId == userId);

        if (node.OwnerId != userId && (userNode == null || userNode.AccessTypeId != AccessTypeId.Write))
        {
            throw new ForbiddenActionException();
        }

        node.Size = file.Length;
        node.ModifiedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        node.ModifiedById = node.OwnerId == userId ? node.OwnerId : userNode.UserId;

        context.Nodes.Update(node);
        await context.SaveChangesAsync();

        var storageResponse = await _storageApiClient.Upload(node.Id, file.OpenReadStream());

        return storageResponse.IsSuccessStatusCode;
    }

    public async Task<Stream?> Download(Guid nodeId, Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);

        if (node == null)
        {
            throw new NodeDoesNotExistException();
        }

        var userNode = await context.UserNodes.FirstOrDefaultAsync(x => x.NodeId == nodeId && x.UserId == userId);

        if (node.OwnerId != userId && userNode == null)
        {
            throw new ForbiddenActionException();
        }

        var storageResponse = await _storageApiClient.Download(nodeId);

        if (storageResponse.IsSuccessStatusCode)
        {
            return await storageResponse.Content.ReadAsStreamAsync();
        }

        return null;
    }
}