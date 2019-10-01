using Gallery.API.Entities;

namespace Gallery.API.Models
{
    public static class GalleryEntityToDtoMapper
    {
        public static GalleryDTO ToGalleryDto(this GalleryEntity galleryEntity, int numberOfImages)
        {
            return new GalleryDTO(galleryEntity.Id, galleryEntity.fk_owner, galleryEntity.Name, numberOfImages);
        }
    }
}
