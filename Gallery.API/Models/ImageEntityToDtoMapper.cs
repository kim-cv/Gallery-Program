using Gallery.API.Entities;

namespace Gallery.API.Models
{
    public static class ImageEntityToDtoMapper
    {
        public static ImageDTO ToImageDto(this ImageEntity imageEntity)
        {
            return new ImageDTO(imageEntity.Id, imageEntity.fk_gallery, imageEntity.Name, imageEntity.Extension, imageEntity.SizeInBytes);
        }
    }
}
