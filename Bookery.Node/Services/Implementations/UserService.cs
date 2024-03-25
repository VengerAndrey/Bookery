using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Data;
using Bookery.Node.Data.Entities;
using Bookery.Node.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Services.Implementations;

public class UserService : IUserService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<UserEntity?> Get(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        return user;
    }
    
    public async Task SignUp(UserSignUpDto userSignUpDto)
    {
        var entity = new UserEntity()
        {
            Id = userSignUpDto.Id,
            Email = userSignUpDto.Email,
            FirstName = userSignUpDto.FirstName,
            LastName = userSignUpDto.LastName
        };

        await using var context = await _contextFactory.CreateDbContextAsync();

        var createdResult = await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();
    }
}