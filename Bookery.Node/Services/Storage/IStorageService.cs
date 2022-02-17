namespace Bookery.Node.Services.Storage;

public interface IStorageService
{
    Task<bool> Upload(Guid id, Stream content);
    Task<Stream> Download(Guid id);
}