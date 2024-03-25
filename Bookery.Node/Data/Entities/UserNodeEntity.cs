using Bookery.Node.Common.Enums;

namespace Bookery.Node.Data.Entities;

public class UserNodeEntity
{
    public NodeEntity? Node { get; set; }
    public Guid NodeId { get; set; }

    public UserEntity? User { get; set; }
    public Guid UserId { get; set; }

    public AccessTypeEntity? AccessType { get; set; }
    public AccessTypeId AccessTypeId { get; set; }

    public long SharedAt { get; set; }
}