namespace Bookery.Common.Exceptions;

public class DomainEventHandlerNotFoundException : Exception
{
    public DomainEventHandlerNotFoundException(Type type) : base($"Domain event handler not found for type '{type}'.")
    {
        
    }
}