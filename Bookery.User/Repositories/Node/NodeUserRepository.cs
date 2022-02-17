using Bookery.User.Entities;

namespace Bookery.User.Repositories.Node;

public class NodeUserRepository : INodeUserRepository
{
    private readonly HttpClient _httpClient;

    public NodeUserRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NodeClient");
    }

    public async Task<bool> Add(NodeUser nodeUser)
    {
        var result = await _httpClient.PostAsJsonAsync("", nodeUser);

        return result.IsSuccessStatusCode;
    }
}