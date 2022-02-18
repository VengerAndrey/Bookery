using Azure.Storage.Blobs;

namespace Bookery.Storage.Services
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
            return response.Status is >= StatusCodes.Status200OK and <= StatusCodes.Status208AlreadyReported;
        }

        public async Task<Stream> Download(Guid id)
        {
            var blobClient = _container.GetBlobClient(id.ToString());
            if (await blobClient.ExistsAsync())
            {
                return await blobClient.OpenReadAsync();
            }
            return Stream.Null;
        }

        public async Task<bool> Delete(Guid id)
        {
            var blobClient = _container.GetBlobClient(id.ToString());
            var result = await blobClient.DeleteIfExistsAsync();
            var response = result.GetRawResponse();
            return response.Status is >= StatusCodes.Status200OK and <= StatusCodes.Status208AlreadyReported;
        }
    }
}
