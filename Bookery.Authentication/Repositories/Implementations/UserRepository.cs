using Bookery.Authentication.Data;
using Bookery.Authentication.Data.Entities;
using Bookery.Authentication.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Authentication.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<UserEntity> Create(UserEntity entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var createdResult = await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();

        return createdResult.Entity;
    }

    public async Task<UserEntity?> GetByEmail(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        return user;
    }
}