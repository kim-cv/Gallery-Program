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
        private static Mock<IUserRepository> userRepository;
        private static List<UserEntity> users = new List<UserEntity>();

        private static Mock<IGalleryRepository> galleryRepository;
        private static Mock<GalleryService> galleryService;
        private static List<GalleryEntity> galleryItems = new List<GalleryEntity>();

        private static Mock<IImageRepository> imageRepository;
        private static List<ImageEntity> imageItems = new List<ImageEntity>();

        private static Mock<IHostingEnvironment> hostingEnvironment;
        private static Mock<IFileSystemRepository> fileSystemRepository;
        private static Mock<ImageService> imageService;

        [ClassInitialize]
        public static void InitTestClass(TestContext testContext)
        {
            // Mock hosting
            string hostingPath = "./apiTestFolder";
            hostingEnvironment = new Mock<IHostingEnvironment>();
            Directory.CreateDirectory(hostingPath);
            hostingEnvironment.SetupGet(x => x.ContentRootPath).Returns(hostingPath);

            // Mock file system repository
            fileSystemRepository = new Mock<IFileSystemRepository>();

            // Mock user repository
            userRepository = new Mock<IUserRepository>();
            userRepository.Setup(repo => repo.GetUser(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string username, string password) =>
            {
                return users.FirstOrDefault(tmpUser => tmpUser.Username == username && tmpUser.Password == password);
            });
            userRepository.Setup(repo => repo.GetUser(It.IsAny<Guid>()))
                .ReturnsAsync((Guid userId) =>
                {
                    return users.FirstOrDefault(tmpUser => tmpUser.Id == userId);
                });
            userRepository.Setup(repo => repo.PostUser(It.IsAny<UserEntity>()))
                .ReturnsAsync((UserEntity userEntity) =>
                {
                    users.Add(userEntity);
                    return userEntity;
                });
            userRepository.Setup(repo => repo.Save())
                .Returns(() =>
                {
                    return true;
                });

            users.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "TestUsername1", Password = "TestPassword1" });
            users.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "TestUsername2", Password = "TestPassword2" });

            // Mock gallery repository
            galleryRepository = new Mock<IGalleryRepository>();
            galleryRepository.Setup(repo => repo.GetGallery(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    return galleryItems.FirstOrDefault(tmpGallery => tmpGallery.Id == galleryId);
                });
            galleryRepository.Setup(repo => repo.GetGalleriesFromOwner(It.IsAny<Guid>(), It.IsAny<Pagination>()))
                .ReturnsAsync((Guid ownerId, Pagination pagination) =>
                {
                    return galleryItems.Where(tmpGallery => tmpGallery.fk_owner == ownerId);
                });
            galleryRepository.Setup(repo => repo.PostGallery(It.IsAny<GalleryEntity>()))
                .ReturnsAsync((GalleryEntity galleryEntity) =>
                {
                    galleryItems.Add(galleryEntity);
                    return galleryEntity;
                });
            galleryRepository.Setup(repo => repo.DeleteGallery(It.IsAny<Guid>()));
            galleryRepository.Setup(repo => repo.Save())
                .Returns(() =>
                {
                    return true;
                });

            galleryItems.Add(new GalleryEntity() { Id = Guid.NewGuid(), fk_owner = users[0].Id, Name = "TestName1" });
            galleryItems.Add(new GalleryEntity() { Id = Guid.NewGuid(), fk_owner = users[0].Id, Name = "TestName2" });

            // Mock image repository
            imageRepository = new Mock<IImageRepository>();
            imageRepository.Setup(repo => repo.GetImage(It.IsAny<Guid>()))
                .ReturnsAsync((Guid imageId) =>
                {
                    return imageItems.FirstOrDefault(tmp => tmp.Id == imageId);
                });
            imageRepository.Setup(repo => repo.PostImage(It.IsAny<ImageEntity>()))
                .ReturnsAsync((ImageEntity imageEntity) =>
                {
                    imageItems.Add(imageEntity);
                    return imageEntity;
                });
            imageRepository.Setup(repo => repo.Save())
                .Returns(() =>
                {
                    return true;
                });

            imageItems.Add(new ImageEntity() { Id = Guid.NewGuid(), fk_gallery = galleryItems[0].Id, gallery = galleryItems[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 });


            galleryService = new Mock<GalleryService>(galleryRepository.Object, imageRepository.Object);

            imageService = new Mock<ImageService>(imageRepository.Object, fileSystemRepository.Object);
        }

        #region GetImages
        [TestMethod]
        public async Task GetImages()
        {
            // Arrange
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());

            Guid galleryId = galleryItems[0].Id;
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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());

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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[1].Id.ToString());

            Guid galleryId = galleryItems[0].Id;
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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());

            Guid imageId = Guid.NewGuid();
            ImageEntity imageEntity = new ImageEntity() { Id = imageId, fk_gallery = galleryItems[0].Id, gallery = galleryItems[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 };
            imageItems.Add(imageEntity);

            // Act
            ActionResult response = await controller.GetImage(galleryItems[0].Id, imageId, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response, typeof(FileContentResult));
            var result = response as FileContentResult;
            //Assert.AreEqual(200, result.StatusCode);
            imageItems.Remove(imageEntity);
        }

        [TestMethod]
        public async Task GetImage_mine_and_image_not_exist()
        {
            // Arrange
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());
            Guid imageId = Guid.NewGuid();

            // Act
            ActionResult response = await controller.GetImage(galleryItems[0].Id, imageId, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetImage_image_not_owned_by_gallery()
        {
            // Arrange
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());
            Guid imageId = Guid.NewGuid();
            ImageEntity imageEntity = new ImageEntity() { Id = imageId, fk_gallery = galleryItems[0].Id, gallery = galleryItems[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 };
            imageItems.Add(imageEntity);

            // Act
            ActionResult response = await controller.GetImage(galleryItems[1].Id, imageId, false, null, null, null);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
            imageItems.Remove(imageEntity);
        }

        [TestMethod]
        public async Task GetImage_not_existing_gallery()
        {
            // Arrange
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());

            Guid galleryId = Guid.NewGuid();
            Guid imageId = imageItems[0].Id;

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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[1].Id.ToString());

            Guid galleryId = galleryItems[0].Id;
            Guid imageId = imageItems[0].Id;

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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());

            ImageUtils utils = new ImageUtils();
            Guid galleryId = galleryItems[0].Id;
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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());

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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[1].Id.ToString());

            Guid galleryId = galleryItems[0].Id;
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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());

            Guid imageId = Guid.NewGuid();
            ImageEntity imageEntity = new ImageEntity() { Id = imageId, fk_gallery = galleryItems[0].Id, gallery = galleryItems[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 };
            imageItems.Add(imageEntity);

            // Act
            ActionResult response = await controller.DeleteImage(galleryItems[0].Id, imageId);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
            var result = response as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);
            imageRepository.Verify(repo => repo.DeleteImage(It.IsAny<Guid>()), Times.Once());
            imageItems.Remove(imageEntity);
        }

        [TestMethod]
        public async Task DeleteImage_mine_and_image_not_exist()
        {
            // Arrange
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());
            Guid imageId = Guid.NewGuid();

            // Act
            ActionResult response = await controller.DeleteImage(galleryItems[0].Id, imageId);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteImage_image_not_owned_by_gallery()
        {
            // Arrange
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());
            Guid imageId = Guid.NewGuid();
            ImageEntity imageEntity = new ImageEntity() { Id = imageId, fk_gallery = galleryItems[0].Id, gallery = galleryItems[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 };
            imageItems.Add(imageEntity);

            // Act
            ActionResult response = await controller.DeleteImage(galleryItems[1].Id, imageId);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
            imageItems.Remove(imageEntity);
        }

        [TestMethod]
        public async Task DeleteImage_not_existing_gallery()
        {
            // Arrange
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[0].Id.ToString());

            Guid galleryId = Guid.NewGuid();
            Guid imageId = imageItems[0].Id;

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
            var controller = new ImageController(galleryService.Object, imageService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(users[1].Id.ToString());

            Guid galleryId = galleryItems[0].Id;
            Guid imageId = imageItems[0].Id;

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
