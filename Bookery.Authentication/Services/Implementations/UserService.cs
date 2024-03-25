using Bookery.Authentication.Common.DTOs.Input;
using Bookery.Authentication.Data.Entities;
using Bookery.Authentication.Repositories.Interfaces;
using Bookery.Authentication.Services.Interfaces;

namespace Bookery.Authentication.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IHasher _hasher;

    public UserService(IUserRepository userRepository, IHasher hasher)
    {
        _userRepository = userRepository;
        _hasher = hasher;
    }
    
    public async Task SignUp(UserSignUpDto userSignUpDto)
    {
        var entity = new UserEntity()
        {
            Id = userSignUpDto.Id,
            Email = userSignUpDto.Email,
            Password = _hasher.Hash(userSignUpDto.Password)
        };

        var createdEntity = await _userRepository.Create(entity);
    }

    public Task<UserEntity?> GetByEmail(string email)
    {
        return _userRepository.GetByEmail(email);
    }
}