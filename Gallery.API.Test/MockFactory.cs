using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Gallery.API.Entities;
using Gallery.API.Helpers;
using Gallery.API.Interfaces;
using Gallery.API.Models;

namespace Gallery.API.Test
{
    public static class MockFactory
    {
        public static Mock<IGalleryService> CreateGalleryServiceMock(List<GalleryEntity> GalleryEntities, List<ImageEntity> ImageEntities)
        {
            Mock<IGalleryService> mock = new Mock<IGalleryService>();

            mock
                .Setup(repo => repo.DoesGalleryExistAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    return (GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId) != null);
                });

            mock
                .Setup(repo => repo.IsGalleryOwnedByUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId, Guid userId) =>
                {
                    GalleryEntity galleryEntity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

                    return galleryEntity.fk_owner == userId;
                });

            mock
                .Setup(repo => repo.CreateGalleryAsync(It.IsAny<Guid>(), It.IsAny<GalleryCreationDTO>()))
                .ReturnsAsync((Guid userId, GalleryCreationDTO galleryCreationDTO) =>
                {
                    GalleryEntity entity = galleryCreationDTO.ToGalleryEntity(userId);
                    GalleryEntities.Add(entity);
                    return entity.ToGalleryDto(0);
                });

            mock
                .Setup(repo => repo.PutGalleryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GalleryPutDTO>()))
                .ReturnsAsync((Guid userId, Guid galleryId, GalleryPutDTO galleryPutDTO) =>
                {
                    GalleryEntity galleryEntity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

                    galleryPutDTO.ToGalleryEntity(ref galleryEntity);

                    int numImagesInGallery = ImageEntities.FindAll(tmp => tmp.fk_gallery == galleryId).Count();

                    return galleryEntity.ToGalleryDto(numImagesInGallery);
                });

            mock
                .Setup(repo => repo.GetGalleryAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    GalleryEntity entity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

                    int numImagesInGallery = ImageEntities.FindAll(tmp => tmp.fk_gallery == galleryId).Count();

                    return entity.ToGalleryDto(numImagesInGallery);
                });

            mock
                .Setup(repo => repo.GetGalleriesByUserAsync(It.IsAny<Guid>(), It.IsAny<Pagination>()))
                .ReturnsAsync((Guid userId, Pagination pagination) =>
                {
                    IEnumerable<GalleryEntity> galleryEntities = GalleryEntities.FindAll(tmp => tmp.fk_owner == userId);

                    IEnumerable<GalleryDTO> galleryDTOs = galleryEntities.Select(tmpEntity =>
                    {
                        int numImagesInGallery = ImageEntities.FindAll(tmp => tmp.fk_gallery == tmpEntity.Id).Count();
                        return tmpEntity.ToGalleryDto(numImagesInGallery);
                    });

                    return galleryDTOs;
                });

            mock
                .Setup(repo => repo.DeleteGalleryAsync(It.IsAny<Guid>()))
                .Callback((Guid galleryId) =>
                {
                    GalleryEntity entity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);
                    GalleryEntities.Remove(entity);
                });

            return mock;
        }
    }
}
