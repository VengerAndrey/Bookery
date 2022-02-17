namespace Bookery.User.Services.Photo;

public interface IPhotoService
{
    Task<bool> UploadProfilePhoto(Guid id, Stream content);
    Task<Stream> DownloadProfilePhoto(Guid id);
}