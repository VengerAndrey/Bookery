using System.Security.Claims;
using Bookery.Domain.DTOs.Responses;

namespace Bookery.Authentication.Services.JWT;

public interface IJwtService
{
    AuthenticationResponse Authenticate(string email, Claim[] claims, DateTime now);
    AuthenticationResponse Refresh(string accessToken, string refreshToken, DateTime now);
    void ClearExpiredRefreshTokens(DateTime now);
    void ClearRefreshToken(string email);
}