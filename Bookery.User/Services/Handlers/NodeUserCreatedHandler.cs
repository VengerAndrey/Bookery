using Bookery.Common.DomainEvents;
using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Common.Client;
using Bookery.User.Data.DomainEvents;

namespace Bookery.User.Services.Handlers;

public class NodeUserCreatedHandler : IDomainEventHandler<NodeUserCreated>
{
    private readonly ILogger<NodeUserCreatedHandler> _logger;
    private readonly INodeApiClient _client;

    private const string ErrorMessagePrefix = $"Could not handle ${nameof(NodeUserCreated)} domain event.";

    public NodeUserCreatedHandler(ILogger<NodeUserCreatedHandler> logger, INodeApiClient client)
    {
        _logger = logger;
        _client = client;
    }
    
    public async Task Handle(NodeUserCreated domainEvent)
    {
        try
        {
            var dto = new UserSignUpDto(
                Id: domainEvent.Id,
                Email: domainEvent.Email,
                FirstName: domainEvent.FirstName,
                LastName: domainEvent.LastName
            );

            var response = await _client.SignUp(dto);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{ErrorMessagePrefix} Bookery.Node returned {response.StatusCode} status code result. Content: ${await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, ErrorMessagePrefix);
        }
    }
}