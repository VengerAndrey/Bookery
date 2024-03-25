using Bookery.Storage.Services.Interfaces;

namespace Bookery.Storage.Services.Implementations;

public class LocalStorageService : IStorageService
{
    private readonly ILogger<LocalStorageService> _logger;
    private readonly string _rootStoragePath = @"./localStorage/data";

    public LocalStorageService(ILogger<LocalStorageService> logger)
    {
        _logger = logger;
        try
        {
            Directory.CreateDirectory(_rootStoragePath);
        }
        catch (Exception e)
        {
            _logger.LogInformation(e, $"Root storage path {_rootStoragePath} could not be created.");
        }
    }

    public async Task<bool> Upload(Guid id, Stream content)
    {
        try
        {
            await using var stream = File.Create(Path.Combine(_rootStoragePath, id.ToString()));
            await content.CopyToAsync(stream);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Task<Stream?> Download(Guid id)
    {
        try
        {
            var stream = File.OpenRead(Path.Combine(_rootStoragePath, id.ToString()));
            return Task.FromResult<Stream?>(stream);
        }
        catch (Exception)
        {
            return Task.FromResult<Stream?>(null);
        }
    }

    public Task<bool> Delete(Guid id)
    {
        try
        {
            File.Delete(Path.Combine(_rootStoragePath, id.ToString()));
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }
}