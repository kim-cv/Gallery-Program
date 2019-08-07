using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Entities
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
    : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
    }
}
