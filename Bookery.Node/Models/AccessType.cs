namespace Bookery.Node.Models;

public class AccessType
{
    public AccessTypeId AccessTypeId { get; set; }
    public string Name { get; set; }
}

public enum AccessTypeId
{
    Read = 0,
    Write = 1
}