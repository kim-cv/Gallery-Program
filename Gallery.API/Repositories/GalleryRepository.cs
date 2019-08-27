using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Helpers;

namespace Gallery.API.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        public readonly GalleryDBContext _context;
        private readonly ILogger<GalleryRepository> _logger;

        public GalleryRepository(GalleryDBContext context, ILogger<GalleryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<GalleryEntity>> GetGalleriesFromOwner(Guid ownerId, Pagination pagination)
        {
            return await _context.Galleries
                .Where(tmpGallery => tmpGallery.fk_owner == ownerId)
                .Include(a => a.owner)
                .Skip(pagination.Skip)
                .Take(pagination.Take)
                .ToListAsync();
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

        public async Task PutGallery(GalleryEntity galleryItem)
        {
            // This repository uses Entity Framework
            // Method is empty because caller is updating entity properties in-place
            // The entity is tracked by EF and reflected onto the DB when calling Save()
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
                _logger.LogError("Uncaught exception during method Save().", ex);
                return false;
            }
        }
    }
}
