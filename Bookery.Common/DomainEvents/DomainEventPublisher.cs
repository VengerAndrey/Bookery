using Bookery.Common.Exceptions;

namespace Bookery.Common.DomainEvents;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Publish(IDomainEvent domainEvent)
    {
        var eventHandlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlerInstance = _serviceProvider.GetService(eventHandlerType) as dynamic;

        if (handlerInstance == null)
        {
            throw new DomainEventHandlerNotFoundException(domainEvent.GetType());
        }

        await handlerInstance.Handle(domainEvent as dynamic);
    }
}