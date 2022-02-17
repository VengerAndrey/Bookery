using Bookery.Node.Data;
using Bookery.Node.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Services.Node;

public class UserService : IUserService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var users = await context.Users.ToListAsync();

        return users;
    }

    public async Task<User> Get(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        return user;
    }

    public async Task<User> Create(User entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var createdResult = await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();

        return createdResult.Entity;
    }

    public async Task<User> Update(User entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Users.Update(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var entity = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        context.Users.Remove(entity);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<User> GetByEmail(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        return user;
    }
}