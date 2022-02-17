using Microsoft.Azure.Cosmos.Table;

namespace Bookery.User.Entities;

public class StsUser : TableEntity
{
    public Guid Id { get; set; }
    public string Password { get; set; }
}