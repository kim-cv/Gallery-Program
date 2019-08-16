using Gallery.API.Entities;

namespace Gallery.API.Models
{
    public static class ImageEntityToDtoMapper
    {
        public static ImageDTO ToImageDto(this ImageEntity imageEntity)
        {
            return new ImageDTO()
            {
                Id = imageEntity.Id,
                galleryId = imageEntity.fk_gallery,
                Name = imageEntity.Name,
                Extension = imageEntity.Extension,
                SizeInBytes = imageEntity.SizeInBytes
            };
        }
    }
}
