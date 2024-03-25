namespace Bookery.Node.Data.Entities;

public class NodeEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsDirectory { get; set; }
    public long? Size { get; set; }

    public NodeEntity? Parent { get; set; }
    public Guid? ParentId { get; set; }

    public UserEntity? Owner { get; set; }
    public Guid OwnerId { get; set; }

    public UserEntity? CreatedBy { get; set; }
    public Guid CreatedById { get; set; }
    public long CreatedAt { get; set; }

    public UserEntity? ModifiedBy { get; set; }
    public Guid ModifiedById { get; set; }
    public long ModifiedAt { get; set; }

    public ICollection<NodeEntity> Children { get; } = new List<NodeEntity>();
    public ICollection<UserNodeEntity> UserNodes { get; set; } = new List<UserNodeEntity>();
}