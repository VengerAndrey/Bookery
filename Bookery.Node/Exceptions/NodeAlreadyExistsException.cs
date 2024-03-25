namespace Bookery.Node.Exceptions;

public class NodeAlreadyExistsException : Exception
{
    public NodeAlreadyExistsException(string name, string? path) : base($"Node with name '{name} already exists for the path '{path ?? string.Empty}'.")
    {
        
    }
}