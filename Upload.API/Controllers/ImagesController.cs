using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Upload.API.Models;
using Upload.API.Models.DTO;
using Upload.API.Repos;

namespace Upload.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IUploadedImageRepo _uploadedImage;

        public ImagesController(IUploadedImageRepo uploadedImage)
        {
            _uploadedImage = uploadedImage;
        }
        // POST: {apibaseurl}/api/images
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file,
            [FromForm] string fileName, [FromForm] string title)
        {
            ValidateFileUpload(file);

            if (ModelState.IsValid)
            {
                // File upload
                var uploadedImage = new UploadedImage
                {
                    FileExtension = Path.GetExtension(file.FileName).ToLower(),
                    FileName = fileName,
                    Title = title,
                    DateCreated = DateTime.Now
                };

                uploadedImage = await _uploadedImage.Upload(file, uploadedImage);

                // Convert Domain Model to DTO
                var response = new UpoadedImageDto
                {
                    Id = uploadedImage.Id,
                    Title = uploadedImage.Title,
                    DateCreated = uploadedImage.DateCreated,
                    FileExtension = uploadedImage.FileExtension,
                    FileName = uploadedImage.FileName,
                    Url = uploadedImage.Url
                };

                return Ok(response);
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(IFormFile file)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file format");
            }

            if (file.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size cannot be more than 10MB");
            }
        }
    }
}
