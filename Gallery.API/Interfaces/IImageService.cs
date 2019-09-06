using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.API.Models;
using Gallery.API.Helpers;

namespace Gallery.API.Interfaces
{
    public interface IImageService
    {
        Task<bool> DoesImageExistAsync(Guid imageId);
        Task<bool> IsImageInsideGalleryAsync(Guid imageId, Guid galleryId);
        Task<ImageDTO> CreateImageAsync(Guid userId, Guid galleryId, ImageCreationDTO imageCreationDTO);
        Task<byte[]> GetImageAsync(Guid imageId, bool thumb, int? thumbWidth, int? thumbHeight, bool? keepAspectRatio);
        Task<IEnumerable<byte[]>> GetImagesInGalleryAsync(Guid galleryId, Pagination pagination, bool thumb, int? thumbWidth, int? thumbHeight, bool? keepAspectRatio);
        Task DeleteImageAsync(Guid imageId);

        byte[] GenerateThumb(byte[] imageData, int maxWidth, int maxHeight, bool keepAspectRatio);
    }
}
