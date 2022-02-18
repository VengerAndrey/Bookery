namespace Bookery.Storage.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _rootPath = @"localStorage/data";

    public LocalStorageService()
    {
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<bool> Upload(Guid id, Stream content)
    {
        try
        {
            await using var stream = File.Create(Path.Combine(_rootPath, id.ToString()));
            await content.CopyToAsync(stream);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public Task<Stream> Download(Guid id)
    {
        try
        {
            var stream = File.OpenRead(Path.Combine(_rootPath, id.ToString()));
            return Task.FromResult(stream as Stream);
        }
        catch (Exception e)
        {
            return Task.FromResult(Stream.Null);
        }
    }

    public Task<bool> Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}