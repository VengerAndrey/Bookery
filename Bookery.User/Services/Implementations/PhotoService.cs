using Bookery.Storage.Common.Client;
using Bookery.User.Services.Interfaces;

namespace Bookery.User.Services.Implementations;

public class PhotoService : IPhotoService
{
    private readonly IStorageApiClient _storageApiClient;

    public PhotoService(IStorageApiClient storageApiClient)
    {
        _storageApiClient = storageApiClient;
    }

    public async Task<bool> UploadProfilePhoto(Guid id, Stream content)
    {
        var response = await _storageApiClient.Upload(id, content);
        return response.IsSuccessStatusCode;
    }

    public async Task<Stream?> DownloadProfilePhoto(Guid id)
    {
        var response = await _storageApiClient.Download(id);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStreamAsync();
        }

        return null;
    }
}