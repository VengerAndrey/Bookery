namespace Bookery.User.Models;

public class UserWithoutPassword
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
}