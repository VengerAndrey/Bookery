namespace Bookery.Domain.DTOs.Responses;

public class AuthenticationResponse
{
    public string Email { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireAt { get; set; }
}