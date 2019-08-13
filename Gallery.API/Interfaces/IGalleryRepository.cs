using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.API.Entities;

namespace Gallery.API.Interfaces
{
    public interface IGalleryRepository
    {
        Task<IEnumerable<GalleryEntity>> GetGalleries();
        Task<IEnumerable<GalleryEntity>> GetGalleriesFromOwner(Guid ownerId);
        Task<GalleryEntity> GetGallery(Guid galleryId);
        Task<GalleryEntity> PostGallery(GalleryEntity galleryEntity);
        Task DeleteGallery(Guid galleryId);
        bool Save();
    }
}
