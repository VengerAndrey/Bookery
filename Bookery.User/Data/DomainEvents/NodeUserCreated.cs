using Bookery.Common.DomainEvents;

namespace Bookery.User.Data.DomainEvents;

public record NodeUserCreated(
    Guid Id,
    string Email,
    string FirstName,
    string LastName
    ) : IDomainEvent;