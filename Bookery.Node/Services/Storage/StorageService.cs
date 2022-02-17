namespace Bookery.Node.Services.Storage;

public class StorageService : IStorageService
{
    private readonly HttpClient _httpClient;

    public StorageService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StorageClient");
    }

    public async Task<bool> Upload(Guid id, Stream content)
    {
        var formData = new MultipartFormDataContent();
        formData.Add(new StreamContent(content), "file", id.ToString());
        var response = await _httpClient.PostAsync(id.ToString(), formData);

        return response.IsSuccessStatusCode;
    }

    public async Task<Stream> Download(Guid id)
    {
        var response = await _httpClient.GetAsync(id.ToString());

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStreamAsync();
        }

        return Stream.Null;
    }
}