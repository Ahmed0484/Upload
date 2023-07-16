using Microsoft.EntityFrameworkCore;
using Upload.API.Models;

namespace Upload.API.Data
{
    public class UploadDbContext : DbContext
    {
        public UploadDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<UploadedImage>  UploadedImages { get; set; }
    }
}
