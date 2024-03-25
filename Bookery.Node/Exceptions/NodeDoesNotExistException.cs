namespace Bookery.Node.Exceptions;

public class NodeDoesNotExistException : Exception
{
    public NodeDoesNotExistException() : base($"Requested node was not found.")
    {
        
    }
}