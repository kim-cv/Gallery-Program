using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class GalleryControllerTest
    {
        // Entities
        private static List<UserEntity> UserEntities = new List<UserEntity>();
        private static List<GalleryEntity> GalleryEntities = new List<GalleryEntity>();
        private static List<ImageEntity> ImageEntities = new List<ImageEntity>();

        // Repositories
        private static Mock<IGalleryRepository> GalleryRepository;
        private static Mock<IImageRepository> ImageRepository;

        // Services
        private static Mock<GalleryService> GalleryService;

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
        }

        #region GetGallery
        [TestMethod]
        public async Task GetGallery_mine_and_it_exist()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            var retrieveThisGalleryItem = GalleryEntities[0];
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            // Act
            ActionResult<GalleryDTO> response = await controller.GetGallery(retrieveThisGalleryItem.Id);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var result = response.Result as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(GalleryDTO));
            GalleryDTO retrievedItem = result.Value as GalleryDTO;
            Assert.AreEqual(retrieveThisGalleryItem.Id, retrievedItem.Id);
            Assert.AreEqual(retrieveThisGalleryItem.Name, retrievedItem.Name);
        }

        [TestMethod]
        public async Task GetGallery_not_mine_and_it_exist()
        {
            //
            GalleryEntity newGallery = await GalleryRepository.Object.PostGallery(new GalleryEntity()
            {
                Id = Guid.NewGuid(),
                fk_owner = UserEntities[1].Id,
                Name = "TestName",
                owner = UserEntities[1]
            });

            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            // Act
            ActionResult<GalleryDTO> response = await controller.GetGallery(newGallery.Id);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(UnauthorizedResult));
            var result = response.Result as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public async Task GetGallery_not_exist()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            // Act
            ActionResult<GalleryDTO> response = await controller.GetGallery(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
            var result = response.Result as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        #endregion

        #region GetGalleries
        [TestMethod]
        public async Task GetGalleries()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            var pagination = new Pagination();

            // Act
            ActionResult<IEnumerable<GalleryDTO>> response = await controller.GetGalleries(pagination);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var result = response.Result as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<GalleryDTO>));
            IEnumerable<GalleryDTO> retrievedItems = result.Value as IEnumerable<GalleryDTO>;
            Assert.AreEqual(2, retrievedItems.Count());
        }
        #endregion

        #region CreateGallery
        [TestMethod]
        public async Task CreateGallery()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[1].Id.ToString());

            GalleryCreationDTO newGalleryItem = new GalleryCreationDTO()
            {
                Name = "CreatedTestName"
            };

            // Act
            ActionResult<GalleryDTO> response = await controller.CreateGallery(newGalleryItem);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(CreatedAtActionResult));
            var result = response.Result as CreatedAtActionResult;
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(GalleryDTO));
            GalleryDTO createdItem = result.Value as GalleryDTO;
            Assert.AreEqual(createdItem.Name, "CreatedTestName");
        }
        #endregion

        #region PutGallery
        [TestMethod]
        public async Task PutGallery_mine_and_it_exist()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            GalleryEntity newGalleryEntity = new GalleryEntity()
            {
                Id = Guid.NewGuid(),
                Name = "TestGalleryName",
                fk_owner = UserEntities[0].Id,
                owner = UserEntities[0]
            };
            GalleryEntities.Add(newGalleryEntity);

            // Act
            GalleryPutDTO galleryPutDto = new GalleryPutDTO()
            {
                Name = "UpdatedTestGalleryName"
            };
            ActionResult<GalleryDTO> response = await controller.PutGallery(newGalleryEntity.Id, galleryPutDto);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(CreatedAtActionResult));
            var result = response.Result as CreatedAtActionResult;
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(GalleryDTO));
            GalleryDTO retrievedItem = result.Value as GalleryDTO;
            Assert.AreEqual(retrievedItem.Id, newGalleryEntity.Id);
            Assert.AreEqual(retrievedItem.Name, "UpdatedTestGalleryName");
            GalleryEntities.Remove(newGalleryEntity);
        }

        [TestMethod]
        public async Task PutGallery_not_mine_and_it_exist()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            GalleryEntity newGalleryEntity = new GalleryEntity()
            {
                Id = Guid.NewGuid(),
                Name = "TestGalleryName",
                fk_owner = UserEntities[1].Id,
                owner = UserEntities[1]
            };
            GalleryEntities.Add(newGalleryEntity);

            // Act
            GalleryPutDTO galleryPutDto = new GalleryPutDTO()
            {
                Name = "UpdatedTestGalleryName"
            };
            ActionResult<GalleryDTO> response = await controller.PutGallery(newGalleryEntity.Id, galleryPutDto);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(UnauthorizedResult));
            var result = response.Result as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public async Task PutGallery_not_exist()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            // Act
            GalleryPutDTO galleryPutDto = new GalleryPutDTO()
            {
                Name = "UpdatedTestGalleryName"
            };
            ActionResult<GalleryDTO> response = await controller.PutGallery(Guid.NewGuid(), galleryPutDto);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
            var result = response.Result as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        #endregion

        #region DeleteGallery
        [TestMethod]
        public async Task DeleteGallery_mine_and_it_exist()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            // Act
            ActionResult response = await controller.DeleteGallery(GalleryEntities[0].Id);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
            var result = response as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);
            GalleryRepository.Verify(repo => repo.DeleteGallery(It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod]
        public async Task DeleteGallery_not_mine_and_it_exist()
        {
            //
            GalleryEntity newGallery = await GalleryRepository.Object.PostGallery(new GalleryEntity()
            {
                Id = Guid.NewGuid(),
                fk_owner = UserEntities[1].Id,
                Name = "TestName",
                owner = UserEntities[1]
            });

            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            // Act
            ActionResult response = await controller.DeleteGallery(newGallery.Id);

            // Assert
            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
            var result = response as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteGallery_not_exist()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

            // Act
            ActionResult response = await controller.DeleteGallery(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        #endregion
    }
}
