using Bookery.Common.Results;
using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Exceptions;
using Bookery.Node.Extensions;
using Bookery.Node.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[ApiController]
[Route("api/Node/Private/{*path}")]
public class PrivateNodeController : ControllerBase
{
    private readonly ILogger<PrivateNodeController> _logger;
    private readonly INodeService _nodeService;

    public PrivateNodeController(ILogger<PrivateNodeController> logger, INodeService nodeService)
    {
        _logger = logger;
        _nodeService = nodeService;
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

            var nodes = await _nodeService.GetByPath(path, userId.Value);

            return new OkObjectResult(nodes);
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(PrivateNodeController)}.{nameof(Get)} call.");
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

            var (createdNode, createdAtPath) = await _nodeService.Create(path, createNodeDto, userId.Value);

            return Created(createdAtPath, createdNode);
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (NodeAlreadyExistsException e)
        {
            return new ConflictObjectResult(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(PrivateNodeController)}.{nameof(Create)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(string? path, [FromBody] UpdateNodeDto updateNodeDto)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }
            
            var (updatedNode, updatedAtPath) = await _nodeService.Update(path, updateNodeDto, userId.Value);

            return Accepted(updatedAtPath, updatedNode);
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (NodeAlreadyExistsException e)
        {
            return new ConflictObjectResult(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(PrivateNodeController)}.{nameof(Update)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string? path)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            await _nodeService.Delete(path, userId.Value);

            return new NoContentResult();
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(PrivateNodeController)}.{nameof(Delete)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}