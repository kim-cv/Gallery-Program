using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.API.Models;
using Gallery.API.Helpers;

namespace Gallery.API.Interfaces
{
    public interface IGalleryService
    {
        Task<bool> DoesGalleryExistAsync(Guid galleryId);
        Task<bool> IsGalleryOwnedByUserAsync(Guid galleryId, Guid userId);
        Task<GalleryDTO> CreateGalleryAsync(Guid userId, GalleryCreationDTO galleryCreationDTO);
        Task<GalleryDTO> PutGalleryAsync(Guid userId, Guid galleryId, GalleryPutDTO galleryPutDTO);
        Task<GalleryDTO> GetGalleryAsync(Guid galleryId);
        Task<IEnumerable<GalleryDTO>> GetGalleriesByUserAsync(Guid userId, Pagination pagination);
        Task DeleteGalleryAsync(Guid galleryId);
    }
}
