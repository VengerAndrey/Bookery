namespace Bookery.Common.DomainEvents;

public interface IDomainEventPublisher
{
    Task Publish(IDomainEvent domainEvent);
}