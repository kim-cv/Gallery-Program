using System;
using System.Threading.Tasks;
using Gallery.API.Entities;
using Gallery.API.Interfaces;

namespace Gallery.API.Repositories
{
    public class ImageRepository : IImageRepository
    {
        public readonly GalleryDBContext _context;

        public ImageRepository(GalleryDBContext context)
        {
            _context = context;
        }

        public async Task<ImageEntity> GetImage(Guid imageId)
        {
            return await _context.Images.FindAsync(imageId);
        }

        public async Task<ImageEntity> PostImage(ImageEntity imageItem)
        {
            var changeTracking = await _context.Images.AddAsync(imageItem);
            return changeTracking.Entity;
        }

        public bool Save()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
