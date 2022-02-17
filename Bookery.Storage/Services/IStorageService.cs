namespace Bookery.Storage.Services;

public interface IStorageService
{
    Task<bool> Upload(Guid id, Stream content);
    Stream Download(Guid id);
}