using System.Text.Json.Serialization;

namespace Bookery.Node.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }

    [JsonIgnore]
    public ICollection<Node> OwnedNodes { get; } = new List<Node>();

    [JsonIgnore]
    public ICollection<Node> ModifiedNodes { get; set; } = new List<Node>();

    [JsonIgnore]
    public ICollection<Node> CreatedNodes { get; set; } = new List<Node>();

    [JsonIgnore]
    public ICollection<UserNode> UserNodes { get; set; } = new List<UserNode>();
}