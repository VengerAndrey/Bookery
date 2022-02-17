using Bookery.Domain.DTOs.Requests;
using Bookery.Domain.DTOs.Responses;
using Bookery.User.Common;
using Bookery.User.Services.Hash;
using Bookery.User.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.User.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SignUpController : ControllerBase
{
    private readonly IHasher _hasher;
    private readonly IUserService _userService;

    public SignUpController(IUserService userService, IHasher hasher)
    {
        _userService = userService;
        _hasher = hasher;
    }

    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest)
    {
        var user = await _userService.GetByEmail(signUpRequest.Email);

        if (user != null)
        {
            return BadRequest(SignUpResult.EmailAlreadyExists);
        }

        if (!EmailValidator.Validate(signUpRequest.Email))
        {
            return BadRequest(SignUpResult.InvalidEmail);
        }

        await _userService.Create(new Models.User
        {
            Email = signUpRequest.Email,
            LastName = signUpRequest.LastName,
            FirstName = signUpRequest.FirstName,
            Password = _hasher.Hash(signUpRequest.Password)
        });

        return Ok(SignUpResult.Success);
    }
}