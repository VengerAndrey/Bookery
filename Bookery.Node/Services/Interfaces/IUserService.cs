using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Data.Entities;

namespace Bookery.Node.Services.Interfaces;

public interface IUserService
{
    Task SignUp(UserSignUpDto userSignUpDto);
    Task<UserEntity?> Get(Guid id);
}