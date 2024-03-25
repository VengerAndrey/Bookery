using Bookery.Common.DomainEvents;

namespace Bookery.User.Data.DomainEvents;

public record AuthenticationUserCreated(
    Guid Id,
    string Email,
    string Password
    ) : IDomainEvent;