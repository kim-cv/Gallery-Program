using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.API.Entities;
using Gallery.API.Helpers;

namespace Gallery.API.Interfaces
{
    public interface IGalleryRepository
    {
        Task<IEnumerable<GalleryEntity>> GetGalleriesFromOwner(Guid ownerId, Pagination pagination);
        Task<GalleryEntity> GetGallery(Guid galleryId);
        Task<GalleryEntity> PostGallery(GalleryEntity galleryEntity);
        Task PutGallery(GalleryEntity galleryEntity);
        Task DeleteGallery(Guid galleryId);
        bool Save();
    }
}
