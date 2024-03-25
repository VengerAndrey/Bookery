using Bookery.Authentication.Common.Client;
using Bookery.Authentication.Common.DTOs.Input;
using Bookery.Common.DomainEvents;
using Bookery.User.Data.DomainEvents;

namespace Bookery.User.Services.Handlers;

public class AuthenticationUserCreatedHandler : IDomainEventHandler<AuthenticationUserCreated>
{
    private readonly ILogger<AuthenticationUserCreatedHandler> _logger;
    private readonly IAuthenticationApiClient _client;

    private const string ErrorMessagePrefix = $"Could not handle ${nameof(AuthenticationUserCreated)} domain event.";

    public AuthenticationUserCreatedHandler(ILogger<AuthenticationUserCreatedHandler> logger, IAuthenticationApiClient client)
    {
        _logger = logger;
        _client = client;
    }
    
    public async Task Handle(AuthenticationUserCreated domainEvent)
    {
        try
        {
            var dto = new UserSignUpDto(
                Id: domainEvent.Id,
                Email: domainEvent.Email,
                Password: domainEvent.Password
                );

            var response = await _client.SignUp(dto);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{ErrorMessagePrefix} Bookery.Authentication returned {response.StatusCode} status code result. Content: ${await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, ErrorMessagePrefix);
        }
    }
}
