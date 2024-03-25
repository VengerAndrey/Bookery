using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Bookery.Authentication.Common.DTOs.Input;
using Bookery.Authentication.Common.DTOs.Output;
using Bookery.Authentication.Data;
using Bookery.Authentication.Data.ValueObjects;
using Bookery.Authentication.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Bookery.Authentication.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly AppConfiguration _appConfiguration;
    private readonly ConcurrentDictionary<string, RefreshTokenValueObject> _refreshTokens = new();

    public JwtService(AppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
    }

    public TokenDto Authenticate(UserClaimsValueObject userClaims)
    {
        var claims = new Claim[]
        {
            new("UserId", userClaims.Id.ToString()),
            new("Email", userClaims.Email)
        };

        var now = DateTime.Now;
        var accessTokenExpireAt = now.AddSeconds(_appConfiguration.Authentication.AccessTokenExpirationInSeconds);
        var refreshTokenExpireAt = now.AddSeconds(_appConfiguration.Authentication.RefreshTokenExpirationInSeconds);

        var jwt = new JwtSecurityToken(
            _appConfiguration.Authentication.Issuer,
            _appConfiguration.Authentication.Audience,
            claims,
            expires: accessTokenExpireAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.Unicode.GetBytes(_appConfiguration.Authentication.SigningKey)),
                SecurityAlgorithms.HmacSha256Signature));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = new RefreshTokenValueObject(
            Email: userClaims.Email,
            Token: GenerateRefreshTokenString(),
            ExpireAt: refreshTokenExpireAt
        );

        _refreshTokens.AddOrUpdate(refreshToken.Token, refreshToken, (_, _) => refreshToken);

        return new TokenDto(
            Email: userClaims.Email,
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token,
            ExpireAt: accessTokenExpireAt
        );
    }

    public TokenDto? Refresh(UserClaimsValueObject userClaims, RefreshTokenDto refreshTokenDto)
    {
        if (!_refreshTokens.TryGetValue(refreshTokenDto.RefreshToken, out var existingRefreshToken))
        {
            return null;
        }

        if (userClaims.Email != existingRefreshToken.Email || existingRefreshToken.ExpireAt < DateTime.Now)
        {
            return null;
        }

        return Authenticate(userClaims);
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
        var refreshTokens = _refreshTokens.Where(x => x.Value.Email == email);

        foreach (var expiredRefreshToken in refreshTokens)
        {
            _refreshTokens.TryRemove(expiredRefreshToken.Key, out _);
        }
    }

    private static string GenerateRefreshTokenString()
    {
        var bytes = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}