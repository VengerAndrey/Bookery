namespace Bookery.User.Services.Interfaces;

public interface IPhotoService
{
    Task<bool> UploadProfilePhoto(Guid id, Stream content);
    Task<Stream?> DownloadProfilePhoto(Guid id);
}