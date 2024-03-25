namespace Bookery.Node.Data.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ICollection<NodeEntity> OwnedNodes { get; set; } = new List<NodeEntity>();
    public ICollection<NodeEntity> ModifiedNodes { get; set; } = new List<NodeEntity>();
    public ICollection<NodeEntity> CreatedNodes { get; set; } = new List<NodeEntity>();
    public ICollection<UserNodeEntity> UserNodes { get; set; } = new List<UserNodeEntity>();
}