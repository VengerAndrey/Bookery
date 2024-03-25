using Bookery.User.Data;
using Bookery.User.Data.Entities;
using Bookery.User.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bookery.User.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<UserEntity>> GetAll()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var users = await context.Users.ToListAsync();

        return users;
    }

    public async Task<UserEntity?> Get(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        return user;
    }

    public async Task<UserEntity> Create(UserEntity entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var createdResult = await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();

        return createdResult.Entity;
    }

    public async Task<UserEntity> Update(UserEntity entity)
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

        if (entity == null)
        {
            return false;
        }
        
        context.Users.Remove(entity);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<UserEntity?> GetByEmail(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        return user;
    }
}