using Bookery.Common.Results;
using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[Route("api/User")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [Route("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] UserSignUpDto userSignUpDto)
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