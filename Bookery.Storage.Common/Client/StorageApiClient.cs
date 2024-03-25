using Bookery.Common.Extensions;

namespace Bookery.Storage.Common.Client;

public class StorageApiClient : IStorageApiClient
{
    private readonly HttpClient _httpClient;

    public StorageApiClient(string baseUrl)
    {
        _httpClient = new()
        {
            BaseAddress = new Uri(baseUrl.WithTrailingSlash())
        };
    }

    public async Task<HttpResponseMessage> Upload(Guid nodeId, Stream content)
    {
        var formData = new MultipartFormDataContent();
        formData.Add(new StreamContent(content), "file", nodeId.ToString());
        var response = await _httpClient.PostAsync($"Storage/{nodeId.ToString()}", formData);

        return response;
    }

    public Task<HttpResponseMessage> Download(Guid nodeId)
    {
        return _httpClient.GetAsync($"Storage/{nodeId.ToString()}");
    }
}