using Upload.API.Models;

namespace Upload.API.Repos
{
    public interface IUploadedImageRepo
    {
        Task<UploadedImage> Upload(IFormFile file, UploadedImage uploadedImage);
    }
}
