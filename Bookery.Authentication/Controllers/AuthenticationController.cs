using System.Security.Claims;
using Bookery.Authentication.Common.DTOs.Input;
using Bookery.Authentication.Data.ValueObjects;
using Bookery.Authentication.Services.Interfaces;
using Bookery.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Authentication.Controllers;

[Route("api/Authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IHasher _hasher;
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;

    public AuthenticationController(ILogger<AuthenticationController> logger, IUserService userService, IJwtService jwtService, IHasher hasher)
    {
        _logger = logger;
        _userService = userService;
        _jwtService = jwtService;
        _hasher = hasher;
    }

    [HttpPost]
    [Route("Token")]
    public async Task<IActionResult> GetToken([FromBody] GetTokenDto getTokenDto)
    {
        try
        {
            var user = await _userService.GetByEmail(getTokenDto.Email);

            var hashedPassword = _hasher.Hash(getTokenDto.Password);

            if (user == null || user.Password != hashedPassword)
            {
                return new UnauthorizedObjectResult("Invalid email or password.");
            }

            var tokenDto = _jwtService.Authenticate(new UserClaimsValueObject(user.Id, user.Email));

            return Ok(tokenDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during AuthenticationController.{nameof(GetToken)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpPost]
    [Route("Token/Refresh")]
    [Authorize]
    public IActionResult RefreshToken([FromBody] RefreshTokenDto refreshDto)
    {
        try
        {
            var principal = Request.HttpContext.User;

            var userIdString = principal.FindFirstValue("UserId");
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return new UnauthorizedResult();
            }
        
            var email = principal.FindFirstValue("Email");
            
            var tokenDto = _jwtService.Refresh(new UserClaimsValueObject(userId, email), refreshDto);

            if (tokenDto == null)
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult(tokenDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(AuthenticationController)}.{nameof(RefreshToken)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpDelete]
    [Route("SignOut")]
    [Authorize]
    public IActionResult SignOutUser()
    {
        try
        {
            var principal = Request.HttpContext.User;
            var email = principal.FindFirstValue("Email");

            _jwtService.ClearRefreshToken(email);

            return new OkResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(AuthenticationController)}.{nameof(SignOutUser)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}