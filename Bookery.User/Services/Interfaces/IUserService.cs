using Bookery.User.Common.DTOs.Input;
using Bookery.User.Common.DTOs.Output;

namespace Bookery.User.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> SignUp(UserSignUpDto userSignUpDto);
    Task<UserDto?> Get(Guid id);
    Task<UserDto?> GetByEmail(string email);
}