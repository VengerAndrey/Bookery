namespace Bookery.User.Services.Photo;

public class PhotoService : IPhotoService
{
    private readonly HttpClient _httpClient;

    public PhotoService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StorageClient");
    }

    public async Task<bool> UploadProfilePhoto(Guid id, Stream content)
    {
        var formData = new MultipartFormDataContent();
        formData.Add(new StreamContent(content), "file", id.ToString());
        var response = await _httpClient.PostAsync(id.ToString(), formData);

        return response.IsSuccessStatusCode;
    }

    public async Task<Stream> DownloadProfilePhoto(Guid id)
    {
        var response = await _httpClient.GetAsync(id.ToString());

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStreamAsync();
        }

        return Stream.Null;
    }
}