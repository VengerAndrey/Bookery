using Bookery.User.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookery.User.Repositories.Main;

public class MainUserRepository : IMainUserRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public MainUserRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<Models.User>> GetAll()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var users = await context.Users.ToListAsync();

        return users;
    }

    public async Task<Models.User> Get(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        return user;
    }

    public async Task<Models.User> Create(Models.User entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var createdResult = await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();

        return createdResult.Entity;
    }

    public async Task<Models.User> Update(Models.User entity)
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

    public async Task<Models.User> GetByEmail(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        return user;
    }
}