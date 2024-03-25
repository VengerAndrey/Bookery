using Bookery.Common.Results;
using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Exceptions;
using Bookery.Node.Extensions;
using Bookery.Node.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookery.Node.Controllers;

[Route("api/Node/Sharing")]
[ApiController]
public class SharingController : ControllerBase
{
    private readonly ILogger<SharingController> _logger;
    private readonly IUserNodeService _userNodeService;

    public SharingController( ILogger<SharingController> logger, IUserNodeService userNodeService)
    {
        _logger = logger;
        _userNodeService = userNodeService;
    }

    [HttpPost]
    [Route("Share")]
    public async Task<IActionResult> Share([FromBody] ShareNodeDto shareNodeDto)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            await _userNodeService.Share(shareNodeDto, userId.Value);

            return new AcceptedResult();
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (ForbiddenActionException)
        {
            return new ForbiddenApiResult();
        }
        catch (InvalidActionException)
        {
            return new BadRequestResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(SharingController)}.{nameof(Share)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpPost]
    [Route("Hide")]
    public async Task<IActionResult> Hide([FromBody] HideNodeDto hideNodeDto)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            await _userNodeService.Hide(hideNodeDto, userId.Value);

            return NoContent();
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (ForbiddenActionException)
        {
            return new ForbiddenApiResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(SharingController)}.{nameof(Hide)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpGet]
    [Route("SharedWith/{id}")]
    public async Task<IActionResult> GetSharedWith(Guid id)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            var users = await _userNodeService.GetSharedWith(id, userId.Value);

            return new OkObjectResult(users);
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (ForbiddenActionException)
        {
            return new ForbiddenApiResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(SharingController)}.{nameof(GetSharedWith)} call.");
            return new InternalServerErrorApiResult();
        }
    }

    [HttpGet]
    [Route("Details/{id}")]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        try
        {
            var userId = Request.GetUserId();

            if (userId == null)
            {
                return new UnauthorizedResult();
            }

            var node = await _userNodeService.GetDetails(id, userId.Value);

            return new OkObjectResult(node);
        }
        catch (NodeDoesNotExistException)
        {
            return new NotFoundResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error occurred during {nameof(SharingController)}.{nameof(GetDetails)} call.");
            return new InternalServerErrorApiResult();
        }
    }
}