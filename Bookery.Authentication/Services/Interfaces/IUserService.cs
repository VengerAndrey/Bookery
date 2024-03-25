using Bookery.Authentication.Common.DTOs.Input;
using Bookery.Authentication.Data.Entities;

namespace Bookery.Authentication.Services.Interfaces;

public interface IUserService
{
    Task SignUp(UserSignUpDto userSignUpDto);
    Task<UserEntity?> GetByEmail(string email);
}