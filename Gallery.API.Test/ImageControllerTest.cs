using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Gallery.API.Controllers;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Models;
using Gallery.TestUtils;
using Gallery.API.Helpers;
using Gallery.API.Services;

namespace Gallery.API.Test
{
    [TestClass]
    public class ImageControllerTest
    {
        // Entities
        private static List<UserEntity> UserEntities = new List<UserEntity>();
        private static List<GalleryEntity> GalleryEntities = new List<GalleryEntity>();
        private static List<ImageEntity> ImageEntities = new List<ImageEntity>();

        // Repositories
        private static Mock<IUserRepository> UserRepository;
        private static Mock<IGalleryRepository> GalleryRepository;
        private static Mock<IImageRepository> ImageRepository;
        private static Mock<IFileSystemRepository> FileSystemRepository;

        // Services
        private static Mock<GalleryService> GalleryService;
        private static Mock<ImageService> ImageService;

        [ClassInitialize]
        public static void InitTestClass(TestContext testContext)
        {
            // Init users entities
            UserEntities.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "TestUsername1", Password = "TestPassword1" });
            UserEntities.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "TestUsername2", Password = "TestPassword2" });


            // Init gallery entities
            GalleryEntities.Add(new GalleryEntity() { Id = Guid.NewGuid(), fk_owner = UserEntities[0].Id, Name = "TestName1" });
            GalleryEntities.Add(new GalleryEntity() { Id = Guid.NewGuid(), fk_owner = UserEntities[0].Id, Name = "TestName2" });


            // Init image entities
            ImageEntities.Add(new ImageEntity() { Id = Guid.NewGuid(), fk_gallery = GalleryEntities[0].Id, gallery = GalleryEntities[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 });


            // Mock file system repository
            FileSystemRepository = new Mock<IFileSystemRepository>();


            // Mock user repository
            UserRepository = new Mock<IUserRepository>();
            UserRepository.Setup(repo => repo.GetUser(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string username, string password) =>
            {
                return UserEntities.FirstOrDefault(tmpUser => tmpUser.Username == username && tmpUser.Password == password);
            });
            UserRepository.Setup(repo => repo.GetUser(It.IsAny<Guid>()))
                .ReturnsAsync((Guid userId) =>
                {
                    return UserEntities.FirstOrDefault(tmpUser => tmpUser.Id == userId);
                });
            UserRepository.Setup(repo => repo.PostUser(It.IsAny<UserEntity>()))
                .ReturnsAsync((UserEntity userEntity) =>
                {
                    UserEntities.Add(userEntity);
                    return userEntity;
                });
            UserRepository.Setup(repo => repo.Save())
                .Returns(() =>
                {
                    return true;
                });


            // Mock gallery repository
            GalleryRepository = new Mock<IGalleryRepository>();
            GalleryRepository.Setup(repo => repo.GetGallery(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    return GalleryEntities.FirstOrDefault(tmpGallery => tmpGallery.Id == galleryId);
                });
            GalleryRepository.Setup(repo => repo.GetGalleriesFromOwner(It.IsAny<Guid>(), It.IsAny<Pagination>()))
                .ReturnsAsync((Guid ownerId, Pagination pagination) =>
                {
                    return GalleryEntities.Where(tmpGallery => tmpGallery.fk_owner == ownerId);
                });
            GalleryRepository.Setup(repo => repo.PostGallery(It.IsAny<GalleryEntity>()))
                .ReturnsAsync((GalleryEntity galleryEntity) =>
                {
                    GalleryEntities.Add(galleryEntity);
                    return galleryEntity;
                });
            GalleryRepository.Setup(repo => repo.DeleteGallery(It.IsAny<Guid>()));
            GalleryRepository.Setup(repo => repo.Save())
                .Returns(() =>
                {
                    return true;
                });


            // Mock image repository
            ImageRepository = new Mock<IImageRepository>();
            ImageRepository.Setup(repo => repo.GetImage(It.IsAny<Guid>()))
                .ReturnsAsync((Guid imageId) =>
                {
                    return ImageEntities.FirstOrDefault(tmp => tmp.Id == imageId);
                });
            ImageRepository.Setup(repo => repo.PostImage(It.IsAny<ImageEntity>()))
                .ReturnsAsync((ImageEntity imageEntity) =>
                {
                    ImageEntities.Add(imageEntity);
                    return imageEntity;
                });
            ImageRepository.Setup(repo => repo.Save())
                .Returns(() =>
                {
                    return true;
                });


            // Mock gallery service
            GalleryService = new Mock<GalleryService>(GalleryRepository.Object, ImageRepository.Object);


            // Mock image service
            ImageService = new Mock<ImageService>(ImageRepository.Object, FileSystemRepository.Object);
        }

        #region GetImages
        [TestMethod]
        public async Task GetImages()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            Guid galleryId = GalleryEntities[0].Id;
            Pagination pagination = new Pagination();

            // Act
            ActionResult<IEnumerable<byte[]>> response = await controller.GetImages(galleryId, pagination, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var result = response.Result as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<byte[]>));
            IEnumerable<byte[]> retrievedItems = result.Value as IEnumerable<byte[]>;
            Assert.AreEqual(0, retrievedItems.Count());
        }

        [TestMethod]
        public async Task GetImages_not_existing_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            Guid galleryId = Guid.NewGuid();
            Pagination pagination = new Pagination();

            // Act
            ActionResult<IEnumerable<byte[]>> response = await controller.GetImages(galleryId, pagination, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
            var result = response.Result as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetImages_not_own_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[1].Id.ToString());

            Guid galleryId = GalleryEntities[0].Id;
            Pagination pagination = new Pagination();

            // Act
            ActionResult<IEnumerable<byte[]>> response = await controller.GetImages(galleryId, pagination, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(UnauthorizedResult));
            var result = response.Result as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }
        #endregion

        #region GetImage
        [TestMethod]
        public async Task GetImage_mine_and_it_exist()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            Guid imageId = Guid.NewGuid();
            ImageEntity imageEntity = new ImageEntity() { Id = imageId, fk_gallery = GalleryEntities[0].Id, gallery = GalleryEntities[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 };
            ImageEntities.Add(imageEntity);

            // Act
            ActionResult response = await controller.GetImage(GalleryEntities[0].Id, imageId, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response, typeof(FileContentResult));
            var result = response as FileContentResult;
            //Assert.AreEqual(200, result.StatusCode);
            ImageEntities.Remove(imageEntity);
        }

        [TestMethod]
        public async Task GetImage_mine_and_image_not_exist()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            Guid imageId = Guid.NewGuid();

            // Act
            ActionResult response = await controller.GetImage(GalleryEntities[0].Id, imageId, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetImage_image_not_owned_by_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            Guid imageId = Guid.NewGuid();
            ImageEntity imageEntity = new ImageEntity() { Id = imageId, fk_gallery = GalleryEntities[0].Id, gallery = GalleryEntities[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 };
            ImageEntities.Add(imageEntity);

            // Act
            ActionResult response = await controller.GetImage(GalleryEntities[1].Id, imageId, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
            ImageEntities.Remove(imageEntity);
        }

        [TestMethod]
        public async Task GetImage_not_existing_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            Guid galleryId = Guid.NewGuid();
            Guid imageId = ImageEntities[0].Id;

            // Act
            ActionResult response = await controller.GetImage(galleryId, imageId, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetImage_not_own_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[1].Id.ToString());

            Guid galleryId = GalleryEntities[0].Id;
            Guid imageId = ImageEntities[0].Id;

            // Act
            ActionResult response = await controller.GetImage(galleryId, imageId, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
            var result = response as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }
        #endregion

        #region CreateImage
        [TestMethod]
        public async Task CreateImage()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            ImageUtils utils = new ImageUtils();
            Guid galleryId = GalleryEntities[0].Id;
            ImageCreationDTO newItem = new ImageCreationDTO()
            {
                Name = "CreatedTestName",
                formFile = utils.TestFormFile()
            };

            // Act
            ActionResult<ImageDTO> response = await controller.CreateImage(galleryId, newItem);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(CreatedAtActionResult));
            var result = response.Result as CreatedAtActionResult;
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(ImageDTO));
            ImageDTO createdItem = result.Value as ImageDTO;
            Assert.AreEqual(createdItem.Name, "CreatedTestName");
        }

        [TestMethod]
        public async Task CreateImage_not_existing_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            Guid galleryId = Guid.NewGuid();
            ImageCreationDTO newItem = new ImageCreationDTO()
            {
                Name = "CreatedTestName",
                formFile = new ImageUtils().TestFormFile()
            };

            // Act
            ActionResult<ImageDTO> response = await controller.CreateImage(galleryId, newItem);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
            var result = response.Result as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task CreateImage_not_own_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[1].Id.ToString());

            Guid galleryId = GalleryEntities[0].Id;
            ImageCreationDTO newItem = new ImageCreationDTO()
            {
                Name = "CreatedTestName",
                formFile = new ImageUtils().TestFormFile()
            };

            // Act
            ActionResult<ImageDTO> response = await controller.CreateImage(galleryId, newItem);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(UnauthorizedResult));
            var result = response.Result as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }
        #endregion

        #region DeleteImage
        [TestMethod]
        public async Task DeleteImage_mine_and_it_exist()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            Guid imageId = Guid.NewGuid();
            ImageEntity imageEntity = new ImageEntity() { Id = imageId, fk_gallery = GalleryEntities[0].Id, gallery = GalleryEntities[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 };
            ImageEntities.Add(imageEntity);

            // Act
            ActionResult response = await controller.DeleteImage(GalleryEntities[0].Id, imageId);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
            var result = response as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);
            ImageRepository.Verify(repo => repo.DeleteImage(It.IsAny<Guid>()), Times.Once());
            ImageEntities.Remove(imageEntity);
        }

        [TestMethod]
        public async Task DeleteImage_mine_and_image_not_exist()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            Guid imageId = Guid.NewGuid();

            // Act
            ActionResult response = await controller.DeleteImage(GalleryEntities[0].Id, imageId);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteImage_image_not_owned_by_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            Guid imageId = Guid.NewGuid();
            ImageEntity imageEntity = new ImageEntity() { Id = imageId, fk_gallery = GalleryEntities[0].Id, gallery = GalleryEntities[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 };
            ImageEntities.Add(imageEntity);

            // Act
            ActionResult response = await controller.DeleteImage(GalleryEntities[1].Id, imageId);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
            ImageEntities.Remove(imageEntity);
        }

        [TestMethod]
        public async Task DeleteImage_not_existing_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            Guid galleryId = Guid.NewGuid();
            Guid imageId = ImageEntities[0].Id;

            // Act
            ActionResult response = await controller.DeleteImage(galleryId, imageId);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteImage_not_own_gallery()
        {
            // Arrange
            var controller = new ImageController(GalleryService.Object, ImageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[1].Id.ToString());

            Guid galleryId = GalleryEntities[0].Id;
            Guid imageId = ImageEntities[0].Id;

            // Act
            ActionResult response = await controller.DeleteImage(galleryId, imageId);

            // Assert
            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
            var result = response as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }
        #endregion
    }
}
