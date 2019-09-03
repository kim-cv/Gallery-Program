using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gallery.API.Entities;
using Gallery.API.Models;
using Gallery.API.Interfaces;
using Gallery.API.Helpers;

namespace Gallery.API.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IImageRepository _imageRepository;

        public GalleryService(IGalleryRepository galleryRepository, IImageRepository imageRepository)
        {
            _galleryRepository = galleryRepository;
            _imageRepository = imageRepository;
        }

        public async Task<bool> DoesGalleryExistAsync(Guid galleryId)
        {
            GalleryEntity gallery = await _galleryRepository.GetGallery(galleryId);
            return (gallery != null);
        }

        public async Task<bool> IsGalleryOwnedByUserAsync(Guid galleryId, Guid userId)
        {
            GalleryEntity gallery = await _galleryRepository.GetGallery(galleryId);
            return (gallery.fk_owner == userId);
        }

        public async Task<GalleryDTO> CreateGalleryAsync(Guid userId, GalleryCreationDTO galleryCreationDTO)
        {
            GalleryEntity entity = galleryCreationDTO.ToGalleryEntity(userId);

            GalleryEntity addedEntity = await _galleryRepository.PostGallery(entity);

            if (_galleryRepository.Save() == false)
            {
                throw new Exception();
            }

            return addedEntity.ToGalleryDto(0);
        }

        public async Task<GalleryDTO> PutGalleryAsync(Guid userId, Guid galleryId, GalleryPutDTO galleryPutDTO)
        {
            GalleryEntity galleryEntity = await _galleryRepository.GetGallery(galleryId);

            galleryPutDTO.ToGalleryEntity(ref galleryEntity);

            await _galleryRepository.PutGallery(galleryEntity);

            if (_galleryRepository.Save() == false)
            {
                throw new Exception();
            }

            int numImagesInGallery = _imageRepository.GetNumberOfImagesInGallery(galleryEntity.Id);
            GalleryDTO dtoToReturn = galleryEntity.ToGalleryDto(numImagesInGallery);

            return dtoToReturn;
        }

        public async Task<GalleryDTO> GetGalleryAsync(Guid galleryId)
        {
            GalleryEntity galleryEntity = await _galleryRepository.GetGallery(galleryId);

            int numImagesInGallery = _imageRepository.GetNumberOfImagesInGallery(galleryEntity.Id);

            return galleryEntity.ToGalleryDto(numImagesInGallery);
        }

        public async Task<IEnumerable<GalleryDTO>> GetGalleriesByUserAsync(Guid userId, Pagination pagination)
        {
            IEnumerable<GalleryEntity> galleryEntities = await _galleryRepository.GetGalleriesFromOwner(userId, pagination);

            IEnumerable<GalleryDTO> galleryDTOs = galleryEntities.Select(tmpEntity =>
            {
                int numImagesInGallery = _imageRepository.GetNumberOfImagesInGallery(tmpEntity.Id);
                return tmpEntity.ToGalleryDto(numImagesInGallery);
            });

            return galleryDTOs;
        }

        public async Task DeleteGalleryAsync(Guid galleryId)
        {
            await _galleryRepository.DeleteGallery(galleryId);

            if (_galleryRepository.Save() == false)
            {
                throw new Exception();
            }
        }
    }
}
