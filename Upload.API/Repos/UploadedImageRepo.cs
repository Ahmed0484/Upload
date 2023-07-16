using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Upload.API.Data;
using Upload.API.Models;

namespace Upload.API.Repos
{
    public class UploadedImageRepo : IUploadedImageRepo
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UploadDbContext _dbContext;

        public UploadedImageRepo(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            UploadDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public async Task<UploadedImage> Upload(IFormFile file, UploadedImage uploadedImage)
        {
            // 1- Upload the Image to API/Images
            var localPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{uploadedImage.FileName}{uploadedImage.FileExtension}");
            using var stream = new FileStream(localPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // 2-Update the database
            // https://codepulse.com/images/somefilename.jpg
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{uploadedImage.FileName}{uploadedImage.FileExtension}";
            uploadedImage.Url = urlPath;

            await _dbContext.UploadedImages.AddAsync(uploadedImage);
            await _dbContext.SaveChangesAsync();

            return uploadedImage;
        }
    }
}
