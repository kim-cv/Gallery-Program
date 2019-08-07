using Gallery.API.Entities;

namespace Gallery.API.Models
{
    public static class GalleryEntityToDtoMapper
    {
        public static GalleryDTO ToGalleryDto(this GalleryEntity galleryEntity)
        {
            return new GalleryDTO()
            {
                Id = galleryEntity.Id,
                ownerId = galleryEntity.fk_owner,
                Name = galleryEntity.Name
            };
        }
    }
}
