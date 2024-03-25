using Azure.Storage.Blobs;
using Bookery.Storage.Services.Interfaces;

namespace Bookery.Storage.Services.Implementations
{
    public class BlobStorageService : IStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(string connectionString, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            _container = blobServiceClient.GetBlobContainerClient(containerName);
            _container.CreateIfNotExists();
        }

        public async Task<bool> Upload(Guid id, Stream content)
        {
            var blobClient = _container.GetBlobClient(id.ToString());
            var result = await blobClient.UploadAsync(content, overwrite: true);
            var response = result.GetRawResponse();
            return IsSuccessStatusCode(response.Status);
        }

        public async Task<Stream?> Download(Guid id)
        {
            var blobClient = _container.GetBlobClient(id.ToString());

            if (await blobClient.ExistsAsync())
            {
                return await blobClient.OpenReadAsync();
            }

            return null;
        }

        public async Task<bool> Delete(Guid id)
        {
            var blobClient = _container.GetBlobClient(id.ToString());
            var result = await blobClient.DeleteIfExistsAsync();
            var response = result.GetRawResponse();
            return IsSuccessStatusCode(response.Status);
        }

        private bool IsSuccessStatusCode(int statusCode) => statusCode is >= 200 and < 300;
    }
}