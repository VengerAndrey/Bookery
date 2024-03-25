namespace Bookery.Node.Exceptions;

public class InsufficientAccessLevelException : Exception
{
    public InsufficientAccessLevelException() : base($"Access level is insufficient to perform an operation.")
    {
        
    }
}