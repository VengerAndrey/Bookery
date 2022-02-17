using Bookery.Node.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Services.Node;

public class NodeService : INodeService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public NodeService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<Models.Node>> GetAll()
    {
        await using var context = _contextFactory.CreateDbContext();

        var items = await context.Nodes.ToListAsync();

        return items;
    }

    public async Task<Models.Node> Get(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var item = await context.Nodes
            .FirstOrDefaultAsync(x => x.Id == id);

        return item;
    }

    public async Task<Models.Node> Create(Models.Node entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        entity.Id = new Guid();

        var createdResult = await context.Nodes.AddAsync(entity);
        await context.SaveChangesAsync();

        return createdResult.Entity;
    }

    public async Task<Models.Node> Update(Models.Node entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Nodes.Update(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var entity = await context.Nodes.FirstOrDefaultAsync(x => x.Id == id);
        context.Nodes.Remove(entity);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}