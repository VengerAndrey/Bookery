using Bookery.Node.Models;
using Bookery.Node.Services.Node;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        var created = await _userService.Create(user);

        if (created is null)
        {
            return Problem();
        }

        return Ok(created);
    }
}