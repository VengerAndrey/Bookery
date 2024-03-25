namespace Bookery.Node.Services.Interfaces;

public interface IStorageService
{
    Task<bool> Upload(Guid nodeId, Guid userId, IFormFile file);
    Task<Stream?> Download(Guid nodeId, Guid userId);
}