using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Entities
{
    public class GalleryContext : DbContext
    {
        public GalleryContext(DbContextOptions<GalleryContext> options)
            : base(options)
        {
        }

        public DbSet<GalleryEntity> GalleryItems { get; set; }
    }
}
