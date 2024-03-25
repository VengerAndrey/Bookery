using Bookery.Common.Results;
using Bookery.User.Common.DTOs.Input;
using Bookery.User.Extensions;
using Bookery.User.Services.Interfaces;
using Bookery.User.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.User.Controllers;

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
    public async Task<IActionResult> SignUp([FromBody] UserSignUpDto userSignUpDto)
    {
        try
        {
            var existingUser = await _userService.GetByEmail(userSignUpDto.Email);

            if (existingUser != null)
            {
                return new ConflictResult();
            }
        
            if (!EmailValidator.Validate(userSignUpDto.Email))
            {
                return new BadRequestResult();
            }

            var createdUser = await _userService.SignUp(userSignUpDto);

            return new OkObjectResult(createdUser);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(UserController)}.{nameof(SignUp)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpGet]
    [Route("Self")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new NotFoundResult();
            }

            var user = await _userService.Get(userId.Value);

            return new OkObjectResult(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(UserController)}.{nameof(Get)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpGet]
    [Route("ByEmail/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        try
        {
            var user = await _userService.GetByEmail(email);

            if (user == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(UserController)}.{nameof(GetByEmail)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpGet]
    [Route("ById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var user = await _userService.Get(id);

            if (user == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(UserController)}.{nameof(GetById)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}