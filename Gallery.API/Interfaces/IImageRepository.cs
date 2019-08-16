using System;
using System.Threading.Tasks;
using Gallery.API.Entities;

namespace Gallery.API.Interfaces
{
    public interface IImageRepository
    {
        Task<ImageEntity> GetImage(Guid imageId);
        Task<ImageEntity> PostImage(ImageEntity imageEntity);
        bool Save();
    }
}
