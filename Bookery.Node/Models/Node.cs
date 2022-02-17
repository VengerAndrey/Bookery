using System.Text.Json.Serialization;

namespace Bookery.Node.Models;

public class Node
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsDirectory { get; set; }
    public long? Size { get; set; }

    [JsonIgnore]
    public Node? Parent { get; set; }

    public Guid? ParentId { get; set; }

    [JsonIgnore]
    public ICollection<Node> Children { get; } = new List<Node>();

    [JsonIgnore]
    public User? Owner { get; set; }

    public Guid OwnerId { get; set; }
    public long CreationTimestamp { get; set; }
    public long ModificationTimestamp { get; set; }

    [JsonIgnore]
    public User? CreatedBy { get; set; }

    public Guid CreatedById { get; set; }

    [JsonIgnore]
    public User? ModifiedBy { get; set; }

    public Guid ModifiedById { get; set; }

    [JsonIgnore]
    public ICollection<UserNode> UserNodes { get; set; } = new List<UserNode>();
}