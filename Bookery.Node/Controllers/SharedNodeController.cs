using Bookery.Common.Results;
using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Exceptions;
using Bookery.Node.Extensions;
using Bookery.Node.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[ApiController]
[Route("api/Node/Shared/{*path}")]
public class SharedNodeController : ControllerBase
{
    private readonly ILogger<SharedNodeController> _logger;
    private readonly IUserNodeService _userNodeService;

    public SharedNodeController(ILogger<SharedNodeController> logger, IUserNodeService userNodeService)
    {
        _logger = logger;
        _userNodeService = userNodeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? path)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            var nodes = await _userNodeService.Get(path, userId.Value);

            return new OkObjectResult(nodes);
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(SharedNodeController)}.{nameof(Get)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(string? path, [FromBody] CreateNodeDto createNodeDto)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            var (createdNode, createdAtPath) = await _userNodeService.Create(path, createNodeDto, userId.Value);

            return Created(createdAtPath, createdNode);
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (NodeAlreadyExistsException)
        {
            return new ConflictResult();
        }
        catch (InsufficientAccessLevelException)
        {
            return new ForbiddenApiResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(SharedNodeController)}.{nameof(Create)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(string path, [FromBody] UpdateNodeDto updateNodeDto)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            var (updatedNode, updatedAtPath) = await _userNodeService.Update(path, updateNodeDto, userId.Value);

            return Accepted(updatedAtPath, updatedNode);
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (NodeAlreadyExistsException)
        {
            return new ConflictResult();
        }
        catch (InsufficientAccessLevelException)
        {
            return new ForbiddenApiResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(SharedNodeController)}.{nameof(Update)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}