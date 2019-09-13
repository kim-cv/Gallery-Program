using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Gallery.API.Controllers;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Models;
using Gallery.TestUtils;
using Gallery.API.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Gallery.API.Test
{
    [TestClass]
    public class ImageControllerTest
    {
        // Entities
        private static List<UserEntity> UserEntities;
        private static List<GalleryEntity> GalleryEntities;
        private static List<ImageEntity> ImageEntities;

        // Services
        private static Mock<IGalleryService> GalleryService;
        private static Mock<IImageService> ImageService;

        [TestInitialize]
        public void InitBeforeEachTest()
        {
            UserEntities = new List<UserEntity>();
            GalleryEntities = new List<GalleryEntity>();
            ImageEntities = new List<ImageEntity>();

            // Init users entities
            UserEntities.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "TestUsername1", Password = "TestPassword1" });
            UserEntities.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "TestUsername2", Password = "TestPassword2" });


            // Init gallery entities
            GalleryEntities.Add(new GalleryEntity() { Id = Guid.NewGuid(), fk_owner = UserEntities[0].Id, Name = "TestName1" });
            GalleryEntities.Add(new GalleryEntity() { Id = Guid.NewGuid(), fk_owner = UserEntities[0].Id, Name = "TestName2" });


            // Init image entities
            ImageEntities.Add(new ImageEntity() { Id = Guid.NewGuid(), fk_gallery = GalleryEntities[0].Id, gallery = GalleryEntities[0], Name = "Test1", Extension = ".jpg", SizeInBytes = 100 });


            // Mock gallery service
            GalleryService = MockFactory.CreateGalleryServiceMock(GalleryEntities, ImageEntities);


            // Mock image service
            ImageService = MockFactory.CreateImageServiceMock(ImageEntities);
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
            Assert.AreEqual(1, retrievedItems.Count());
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

            Guid galleryId = GalleryEntities[0].Id;
            ImageCreationDTO newItem = new ImageCreationDTO()
            {
                Name = "CreatedTestName",
                formFile = TestFormFile()
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
                formFile = TestFormFile()
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
                formFile = TestFormFile()
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
            ImageService.Verify(repo => repo.DeleteImageAsync(It.IsAny<Guid>()), Times.Once());
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

        private IFormFile TestFormFile()
        {
            string text = "This is a dummy file for unit testing";
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt");
        }
    }
}
