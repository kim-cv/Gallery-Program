﻿using Gallery.API.Entities;

namespace Gallery.API.Models
{
    public static class GalleryDtoToEntityMapper
    {
        public static GalleryEntity ToGalleryEntity(this GalleryDTO galleryDto)
        {
            return new GalleryEntity()
            {
                Id = galleryDto.Id,
                fk_owner = galleryDto.ownerId,
                Name = galleryDto.Name
            };
        }

        public static GalleryEntity ToGalleryEntity(this GalleryCreationDTO galleryDto)
        {
            return new GalleryEntity()
            {
                Name = galleryDto.Name,
                fk_owner = galleryDto.ownerId
            };
        }

        /// <summary>
        /// Updates the entity properties in-place
        /// </summary>
        public static void ToGalleryEntity(this GalleryPutDTO galleryDto, ref GalleryEntity entityToUpdate)
        {
            entityToUpdate.Name = galleryDto.Name;
        }
    }
}
