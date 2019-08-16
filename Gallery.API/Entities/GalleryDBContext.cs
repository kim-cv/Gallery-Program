using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Entities
{
    public class GalleryDBContext : DbContext
    {
        public GalleryDBContext(DbContextOptions<GalleryDBContext> options)
            : base(options)
        {
        }

        public DbSet<GalleryEntity> Galleries { get; set; }
        public DbSet<ImageEntity> Images { get; set; }
        public DbSet<UserEntity> Users { get; set; }
    }
}
