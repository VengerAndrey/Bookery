using System.Security.Claims;
using Bookery.Authentication.Repositories.User;
using Bookery.Authentication.Services.Common;
using Bookery.Authentication.Services.Hash;
using Bookery.Authentication.Services.JWT;
using Bookery.Domain.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Bookery.Authentication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IHasher _hasher;
    private readonly IHeaderService _headerService;
    private readonly IJwtService _jwtService;
    private readonly IStsUserRepository _stsUserRepository;

    public AuthenticationController(IJwtService jwtService, IStsUserRepository stsUserRepository, IHasher hasher,
        IHeaderService headerService)
    {
        _jwtService = jwtService;
        _stsUserRepository = stsUserRepository;
        _hasher = hasher;
        _headerService = headerService;
    }

    [HttpPost]
    [Route("token")]
    public async Task<IActionResult> Token([FromBody] AuthenticationRequest authenticationRequest)
    {
        var identity = await GetIdentity(authenticationRequest);

        if (identity is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var user = await _stsUserRepository.GetByEmail(authenticationRequest.Email);

        if (user is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Email", user.RowKey)
        };

        var response = _jwtService.Authenticate(authenticationRequest.Email, claims, DateTime.UtcNow);

        return Ok(response);
    }

    [HttpPost]
    [Route("refresh-token")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
    {
        var authorizationHeader = Request.Headers[HeaderNames.Authorization];
        if (authorizationHeader.Count == 0)
        {
            return Unauthorized("Invalid token.");
        }

        var bearer = authorizationHeader[0];
        if (!bearer.Contains("Bearer "))
        {
            return Unauthorized("Invalid token.");
        }

        var accessToken = bearer.Replace("Bearer ", "");
        var response = _jwtService.Refresh(accessToken, refreshRequest.RefreshToken, DateTime.UtcNow);

        if (response is null)
        {
            return Unauthorized("Invalid token.");
        }

        return Ok(response);
    }

    [HttpDelete]
    [Route("sign-out")]
    public async Task<IActionResult> SignOut()
    {
        var stsUser = await _headerService.GetRequestUser(Request);
        if (stsUser is null)
        {
            return BadRequest();
        }

        Task.Run(() => _jwtService.ClearRefreshToken(stsUser.RowKey));

        return Ok();
    }

    private async Task<ClaimsIdentity?> GetIdentity(AuthenticationRequest authenticationRequest)
    {
        var stsUser = await _stsUserRepository.GetByEmail(authenticationRequest.Email);

        var hashedPassword = _hasher.Hash(authenticationRequest.Password);

        if (stsUser is null || stsUser.Password != hashedPassword)
        {
            return null;
        }

        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, stsUser.RowKey),
            new(ClaimsIdentity.DefaultRoleClaimType, "DefaultRole")
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }
}