using Bookery.User.Data.Entities;

namespace Bookery.User.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByEmail(string email);
    
    Task<List<UserEntity>> GetAll();
    Task<UserEntity?> Get(Guid id);
    Task<UserEntity> Create(UserEntity entity);
    Task<UserEntity> Update(UserEntity entity);
    Task<bool> Delete(Guid id);
}