namespace Bookery.Storage.Services;

public interface IStorageService
{
    Task<bool> Upload(Guid id, Stream content);
    Task<Stream> Download(Guid id);

    Task<bool> Delete(Guid id);
}