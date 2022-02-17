namespace Bookery.Domain.DTOs.Requests;

public class AuthenticationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}