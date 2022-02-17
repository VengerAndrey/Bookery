using Bookery.User.Models;
using Bookery.User.Services.Common;
using Bookery.User.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.User.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IHeaderService _headerService;
    private readonly IUserService _userService;

    public UserController(IUserService userService, IHeaderService headerService)
    {
        _userService = userService;
        _headerService = headerService;
    }

    [HttpGet]
    [Route("self")]
    public async Task<IActionResult> Get()
    {
        var user = await _headerService.GetRequestUser(Request);

        if (user is null)
        {
            return NotFound();
        }

        var userWithoutPassword = new UserWithoutPassword
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return new JsonResult(userWithoutPassword);
    }

    [HttpGet]
    [Route("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var user = await _userService.GetByEmail(email);

        if (user is null)
        {
            return NotFound();
        }

        var userWithoutPassword = new UserWithoutPassword
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return new JsonResult(userWithoutPassword);
    }

    [HttpGet]
    [Route("id/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.Get(id);

        if (user is null)
        {
            return NotFound();
        }

        var userWithoutPassword = new UserWithoutPassword
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return new JsonResult(userWithoutPassword);
    }
}