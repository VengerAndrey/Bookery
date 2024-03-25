using System.Net.Http.Json;
using Bookery.Common.Extensions;
using Bookery.Node.Common.DTOs.Input;

namespace Bookery.Node.Common.Client;

public class NodeApiClient : INodeApiClient
{
    private readonly HttpClient _httpClient;

    public NodeApiClient(string baseUrl)
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