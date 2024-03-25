using Bookery.Common.DomainEvents;
using Bookery.User.Common.DTOs.Input;
using Bookery.User.Data.DomainEvents;
using Bookery.User.Data.Entities;

namespace Bookery.User.Data.AggregateRoots;

public class UserAggregateRoot
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents;
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public UserAggregateRoot(UserSignUpDto userSignUpDto)
    {
        Id = Guid.NewGuid();
        Email = userSignUpDto.Email;
        Password = userSignUpDto.Password;
        FirstName = userSignUpDto.FirstName;
        LastName = userSignUpDto.LastName;
        
        _domainEvents.Add(new AuthenticationUserCreated(
            Id: Id,
            Email: Email,
            Password: Password));
        
        _domainEvents.Add(new NodeUserCreated(
            Id: Id,
            Email: Email,
            FirstName: FirstName,
            LastName: LastName
            ));
    }

    public UserEntity ToEntity() =>
        new()
        {
            Id = Id,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName
        };
}