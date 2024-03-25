using Bookery.Authentication.Common.DTOs.Input;
using Bookery.Authentication.Services.Interfaces;
using Bookery.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Authentication.Controllers;

[Route("api/User")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost]
    [Route("SignUp")]
    public async Task<IActionResult> SignUp(UserSignUpDto userSignUpDto)
    {
        try
        {
            await _userService.SignUp(userSignUpDto);

            return new OkResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(UserController)}.{nameof(SignUp)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}