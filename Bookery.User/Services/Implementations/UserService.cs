using Bookery.Common.DomainEvents;
using Bookery.Common.DomainEvents.Extensions;
using Bookery.User.Common.DTOs.Input;
using Bookery.User.Common.DTOs.Output;
using Bookery.User.Data.AggregateRoots;
using Bookery.User.Mappers;
using Bookery.User.Repositories.Interfaces;
using Bookery.User.Services.Interfaces;

namespace Bookery.User.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public UserService(IUserRepository userRepository, IDomainEventPublisher domainEventPublisher)
    {
        _userRepository = userRepository;
        _domainEventPublisher = domainEventPublisher;
    }

    public async Task<UserDto> SignUp(UserSignUpDto userSignUpDto)
    {
        var aggregateRoot = new UserAggregateRoot(userSignUpDto);
        
        var createdEntity = await _userRepository.Create(aggregateRoot.ToEntity());

        await _domainEventPublisher.PublishManyParallel(aggregateRoot.DomainEvents);

        return UserMapper.ToDto(createdEntity);
    }

    public async Task<UserDto?> Get(Guid id)
    {
        var entity = await _userRepository.Get(id);

        return entity == null ? null : UserMapper.ToDto(entity);
    }

    public async Task<UserDto?> GetByEmail(string email)
    {
        var entity = await _userRepository.GetByEmail(email);

        return entity == null ? null : UserMapper.ToDto(entity);
    }
}