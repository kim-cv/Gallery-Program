using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.API.Entities;

namespace Gallery.API.Interfaces
{
    public interface IImageRepository
    {
        Task<IEnumerable<ImageEntity>> GetImages(Guid galleryId);
        Task<ImageEntity> GetImage(Guid imageId);
        Task<ImageEntity> PostImage(ImageEntity imageEntity);
        Task DeleteImage(Guid imageId);
        bool Save();
    }
}
