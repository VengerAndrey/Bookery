namespace Bookery.Common.DomainEvents.Extensions;

public static class DomainEventPublisherExtensions
{
    public static async Task PublishManySerial(this IDomainEventPublisher domainEventPublisher, IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await domainEventPublisher.Publish(domainEvent);
        }
    }
    
    public static Task PublishManyParallel(this IDomainEventPublisher domainEventPublisher, IEnumerable<IDomainEvent> domainEvents)
    {
        var tasks = domainEvents.Select(domainEventPublisher.Publish);
        return Task.WhenAll(tasks);
    }
}