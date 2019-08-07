using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Gallery.API.Entities;
using Gallery.API.Interfaces;

namespace Gallery.API.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        public readonly GalleryContext _context;

        public GalleryRepository(GalleryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GalleryEntity>> GetGalleries()
        {
            return await _context.GalleryItems.ToListAsync();
        }

        public async Task<GalleryEntity> GetGallery(Guid galleryId)
        {
            return await _context.GalleryItems.FindAsync(galleryId);
        }

        public async Task<GalleryEntity> PostGallery(GalleryEntity galleryItem)
        {
            var changeTracking = await _context.GalleryItems.AddAsync(galleryItem);
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
