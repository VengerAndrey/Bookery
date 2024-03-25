namespace Bookery.Node.Exceptions;

public class ForbiddenActionException : Exception
{
    public ForbiddenActionException() : base ($"Action is forbidden.")
    {
        
    }
}