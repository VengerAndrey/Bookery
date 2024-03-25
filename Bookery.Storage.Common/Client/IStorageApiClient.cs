namespace Bookery.Storage.Common.Client;

public interface IStorageApiClient
{
    Task<HttpResponseMessage> Upload(Guid nodeId, Stream content);
    Task<HttpResponseMessage> Download(Guid nodeId);
}