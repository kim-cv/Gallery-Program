using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Helpers;

namespace Gallery.API.Repositories
{
    public class ImageRepository : IImageRepository
    {
        public readonly GalleryDBContext _context;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(GalleryDBContext context, ILogger<ImageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ImageEntity>> GetImages(Guid galleryId, Pagination pagination)
        {
            return await _context
                .Images
                .Where(tmpImage => tmpImage.fk_gallery == galleryId)
                .Skip(pagination.Skip)
                .Take(pagination.Take)
                .ToListAsync();
        }

        public int GetNumberOfImagesInGallery(Guid galleryId)
        {
            return _context.Images.Where(tmpImage => tmpImage.fk_gallery == galleryId).Count();
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

        public async Task DeleteImage(Guid imageId)
        {
            var imageEntity = await GetImage(imageId);

            if (imageEntity == null)
            {
                return;
            }

            _context.Images.Remove(imageEntity);

            return;
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
                _logger.LogError("Uncaught exception during method Save().", ex);
                return false;
            }
        }
    }
}
