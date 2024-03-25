namespace Bookery.Node.Exceptions;

public class InvalidActionException : Exception
{
    public InvalidActionException() : base($"Action is not permitted.")
    {
        
    }
}