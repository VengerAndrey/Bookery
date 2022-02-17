using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Bookery.Config;
using Bookery.Domain.DTOs;
using Bookery.Domain.DTOs.Responses;
using Microsoft.IdentityModel.Tokens;

namespace Bookery.Authentication.Services.JWT;

public class JwtService : IJwtService
{
    private readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens = new();

    public AuthenticationResponse Authenticate(string email, Claim[] claims, DateTime now)
    {
        var needAudience =
            string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);

        var accessTokenExpireAt = now.AddSeconds(AuthConfig.AccessTokenExpiration);

        var jwt = new JwtSecurityToken(
            AuthConfig.Issuer,
            needAudience ? AuthConfig.Audience : string.Empty,
            claims,
            expires: accessTokenExpireAt,
            signingCredentials: new SigningCredentials(AuthConfig.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256Signature));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = new RefreshToken
        {
            Email = email,
            Token = GenerateRefreshTokenString(),
            ExpireAt = now.AddSeconds(AuthConfig.RefreshTokenExpiration)
        };

        _refreshTokens.AddOrUpdate(refreshToken.Token, refreshToken, (s, dto) => refreshToken);

        return new AuthenticationResponse
        {
            Email = email,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpireAt = accessTokenExpireAt
        };
    }

    public AuthenticationResponse Refresh(string accessToken, string refreshToken, DateTime now)
    {
        var (principal, jwt) = DecodeJwt(accessToken);

        if (jwt is null || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
        {
            return null;
        }

        var email = principal.FindFirstValue("Email");

        if (!_refreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
        {
            return null;
        }

        if (email != existingRefreshToken.Email || existingRefreshToken.ExpireAt < now)
        {
            return null;
        }

        return Authenticate(email, principal.Claims.ToArray(), now);
    }

    public void ClearExpiredRefreshTokens(DateTime now)
    {
        var expiredRefreshTokens = _refreshTokens.Where(x => x.Value.ExpireAt < now).ToList();

        foreach (var expiredRefreshToken in expiredRefreshTokens)
        {
            _refreshTokens.TryRemove(expiredRefreshToken.Key, out _);
        }
    }

    public void ClearRefreshToken(string email)
    {
        var refreshTokens = _refreshTokens.Where(x => x.Value.Email == email).ToList();

        foreach (var expiredRefreshToken in refreshTokens)
        {
            _refreshTokens.TryRemove(expiredRefreshToken.Key, out _);
        }
    }

    private static (ClaimsPrincipal, JwtSecurityToken) DecodeJwt(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (null, null);
        }

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = AuthConfig.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = AuthConfig.GetSymmetricSecurityKey(),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        return (principal, validatedToken as JwtSecurityToken);
    }

    private static string GenerateRefreshTokenString()
    {
        var bytes = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}