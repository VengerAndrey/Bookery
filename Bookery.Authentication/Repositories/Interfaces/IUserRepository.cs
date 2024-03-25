using Bookery.Authentication.Data.Entities;

namespace Bookery.Authentication.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserEntity> Create(UserEntity entity);
    Task<UserEntity?> GetByEmail(string email);
}