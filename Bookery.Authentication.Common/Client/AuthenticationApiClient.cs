using System.Net.Http.Json;
using Bookery.Authentication.Common.DTOs.Input;
using Bookery.Common.Extensions;

namespace Bookery.Authentication.Common.Client;

public class AuthenticationApiClient : IAuthenticationApiClient
{
    private readonly HttpClient _httpClient;

    public AuthenticationApiClient(string baseUrl)
    {
        _httpClient = new()
        {
            BaseAddress = new Uri(baseUrl.WithTrailingSlash())
        };
    }
    
    public Task<HttpResponseMessage> SignUp(UserSignUpDto userSignUpDto)
    {
        return _httpClient.PostAsJsonAsync("User/SignUp", userSignUpDto);
    }
}