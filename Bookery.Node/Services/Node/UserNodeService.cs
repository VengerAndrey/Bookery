using Bookery.Node.Data;
using Bookery.Node.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Services.Node;

public class UserNodeService : IUserNodeService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserNodeService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<UserNode>> GetAll()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var items = await context.UserNodes.ToListAsync();

        return items;
    }

    public async Task<UserNode> Create(UserNode entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var createdResult = await context.UserNodes.AddAsync(entity);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return null;
        }

        return createdResult.Entity;
    }

    public async Task<UserNode> Update(UserNode entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.UserNodes.Update(entity);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return null;
        }

        return entity;
    }

    public async Task<bool> Delete(UserNode entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.UserNodes.Remove(entity);
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