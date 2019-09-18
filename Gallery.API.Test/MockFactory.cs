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
                .Setup(service => service.DoesGalleryExistAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    return (GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId) != null);
                });

            mock
                .Setup(service => service.IsGalleryOwnedByUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId, Guid userId) =>
                {
                    GalleryEntity galleryEntity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

                    return galleryEntity.fk_owner == userId;
                });

            mock
                .Setup(service => service.CreateGalleryAsync(It.IsAny<Guid>(), It.IsAny<GalleryCreationDTO>()))
                .ReturnsAsync((Guid userId, GalleryCreationDTO galleryCreationDTO) =>
                {
                    GalleryEntity entity = galleryCreationDTO.ToGalleryEntity(userId);
                    GalleryEntities.Add(entity);
                    return entity.ToGalleryDto(0);
                });

            mock
                .Setup(service => service.PutGalleryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GalleryPutDTO>()))
                .ReturnsAsync((Guid userId, Guid galleryId, GalleryPutDTO galleryPutDTO) =>
                {
                    GalleryEntity galleryEntity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

                    galleryPutDTO.ToGalleryEntity(ref galleryEntity);

                    int numImagesInGallery = ImageEntities.FindAll(tmp => tmp.fk_gallery == galleryId).Count();

                    return galleryEntity.ToGalleryDto(numImagesInGallery);
                });

            mock
                .Setup(service => service.GetGalleryAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    GalleryEntity entity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

                    int numImagesInGallery = ImageEntities.FindAll(tmp => tmp.fk_gallery == galleryId).Count();

                    return entity.ToGalleryDto(numImagesInGallery);
                });

            mock
                .Setup(service => service.GetGalleriesByUserAsync(It.IsAny<Guid>(), It.IsAny<Pagination>()))
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
                .Setup(service => service.DeleteGalleryAsync(It.IsAny<Guid>()))
                .Callback((Guid galleryId) =>
                {
                    GalleryEntity entity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);
                    GalleryEntities.Remove(entity);
                });

            return mock;
        }

        public static Mock<IImageService> CreateImageServiceMock(List<ImageEntity> ImageEntities)
        {
            Mock<IImageService> mock = new Mock<IImageService>();

            mock
                .Setup(service => service.DoesImageExistAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid imageId) =>
                {
                    ImageEntity image = ImageEntities.FirstOrDefault(tmp => tmp.Id == imageId);
                    return (image != null);
                });

            mock
                .Setup(service => service.IsImageInsideGalleryAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid imageId, Guid galleryId) =>
                {
                    ImageEntity image = ImageEntities.FirstOrDefault(tmp => tmp.Id == imageId);
                    return (image.fk_gallery == galleryId);
                });

            mock
                .Setup(service => service.CreateImageAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<ImageCreationDTO>()))
                .ReturnsAsync((Guid userId, Guid galleryId, ImageCreationDTO imageCreationDTO) =>
                {
                    ImageEntity entity = imageCreationDTO.ToImageEntity();
                    entity.fk_gallery = galleryId;
                    ImageEntities.Add(entity);
                    return entity.ToImageDto();
                });

            mock
                .Setup(service => service.GetImageAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>()))
                .ReturnsAsync((Guid imageId, bool thumb, int? thumbWidth, int? thumbHeight, bool? keepAspectRatio) =>
                {
                    ImageEntity entity = ImageEntities.FirstOrDefault(tmp => tmp.Id == imageId);
                    return new byte[entity.SizeInBytes];
                });

            mock
                .Setup(service => service.GetImagesInGalleryAsync(It.IsAny<Guid>(), It.IsAny<Pagination>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool?>()))
                .ReturnsAsync((Guid galleryId, Pagination pagination, bool thumb, int? thumbWidth, int? thumbHeight, bool? keepAspectRatio) =>
                {
                    IEnumerable<ImageEntity> imageEntities = ImageEntities.FindAll(tmp => tmp.fk_gallery == galleryId);

                    IEnumerable<byte[]> imgData = imageEntities.Select(tmpEntity =>
                    {
                        return new byte[tmpEntity.SizeInBytes];
                    });

                    return imgData;
                });

            mock
                .Setup(service => service.DeleteImageAsync(It.IsAny<Guid>()))
                .Callback((Guid imageId) =>
                {
                    ImageEntity entity = ImageEntities.FirstOrDefault(tmp => tmp.Id == imageId);
                    ImageEntities.Remove(entity);
                });

            return mock;
        }

        public static Mock<IUserService> CreateUserServiceMock(List<UserEntity> UserEntities)
        {
            Mock<IUserService> mock = new Mock<IUserService>();

            mock
                .Setup(service => service.DoesUserExist(It.IsAny<Guid>()))
                .ReturnsAsync((Guid userId) =>
                {
                    return (UserEntities.FirstOrDefault(tmp => tmp.Id == userId) != null);
                });

            mock
                .Setup(service => service.DoesUserExist(It.IsAny<string>()))
                .ReturnsAsync((string username) =>
                {
                    return (UserEntities.FirstOrDefault(tmp => tmp.Username == username) != null);
                });

            mock
                .Setup(service => service.CreateUserAsync(It.IsAny<UserCreationDTO>()))
                .ReturnsAsync((UserCreationDTO userCreationDTO) =>
                {
                    UserEntity entity = userCreationDTO.ToUserEntity();
                    UserEntities.Add(entity);
                    return entity.ToUserDto();
                });

            mock
                .Setup(service => service.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) =>
                {
                    UserEntity entity = UserEntities.FirstOrDefault(tmpUser => tmpUser.Id == id);
                    return entity.ToUserDto();
                });

            mock
                .Setup(service => service.GetUserAsync(It.IsAny<string>()))
                .Returns((string username) =>
                {
                    UserEntity entity = UserEntities.FirstOrDefault(tmpUser => tmpUser.Username == username);
                    return entity.ToUserDto();
                });

            mock
                .Setup(service => service.LoginAsync(It.IsAny<UserLoginDTO>()))
                .ReturnsAsync((UserLoginDTO userLoginDTO) =>
                {
                    UserEntity entity = UserEntities.First(tmp => tmp.Username == userLoginDTO.username);
                    
                    if (userLoginDTO.password != entity.Password)
                    {
                        throw new Exception("Wrong Password");
                    }

                    return Guid.NewGuid().ToString();
                });

            return mock;
        }
    }
}
