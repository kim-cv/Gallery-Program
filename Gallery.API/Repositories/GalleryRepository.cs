using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using System.Linq;

namespace Gallery.API.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        public readonly GalleryDBContext _context;

        public GalleryRepository(GalleryDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GalleryEntity>> GetGalleriesFromOwner(Guid ownerId)
        {
            return await _context.Galleries.Where(tmpGallery => tmpGallery.fk_owner == ownerId).Include(a => a.owner).ToListAsync();
        }

        public async Task<GalleryEntity> GetGallery(Guid galleryId)
        {
            return await _context.Galleries.FindAsync(galleryId);
        }

        public async Task<GalleryEntity> PostGallery(GalleryEntity galleryItem)
        {
            var changeTracking = await _context.Galleries.AddAsync(galleryItem);
            return changeTracking.Entity;
        }

        public async Task DeleteGallery(Guid galleryId)
        {
            var galleryEntity = await GetGallery(galleryId);

            if (galleryEntity == null)
            {
                return;
            }

            _context.Galleries.Remove(galleryEntity);

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
                return false;
            }
        }
    }
}
