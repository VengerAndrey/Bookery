using Bookery.Node.Common.Enums;

namespace Bookery.Node.Data.Entities;

public class AccessTypeEntity
{
    public AccessTypeId AccessTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
}