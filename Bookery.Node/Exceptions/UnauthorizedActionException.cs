namespace Bookery.Node.Exceptions;

public class UnauthorizedActionException : Exception
{
    public UnauthorizedActionException() : base($"Action is unauthorized.")
    {
        
    }
}