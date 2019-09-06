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

namespace Gallery.API.Test
{
    [TestClass]
    public class GalleryControllerTest
    {
        // Entities
        private static List<UserEntity> UserEntities = new List<UserEntity>();
        private static List<GalleryEntity> GalleryEntities = new List<GalleryEntity>();
        private static List<ImageEntity> ImageEntities = new List<ImageEntity>();

        // Services
        private static Mock<IGalleryService> GalleryService;

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


            // Mock gallery service
            GalleryService = new Mock<IGalleryService>();

            GalleryService
                .Setup(repo => repo.DoesGalleryExistAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    return (GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId) != null);
                });

            GalleryService
                .Setup(repo => repo.IsGalleryOwnedByUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId, Guid userId) =>
                {
                    GalleryEntity galleryEntity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

                    return galleryEntity.fk_owner == userId;
                });

            GalleryService
                .Setup(repo => repo.CreateGalleryAsync(It.IsAny<Guid>(), It.IsAny<GalleryCreationDTO>()))
                .ReturnsAsync((Guid userId, GalleryCreationDTO galleryCreationDTO) =>
                {
                    GalleryEntity entity = galleryCreationDTO.ToGalleryEntity(userId);
                    GalleryEntities.Add(entity);
                    return entity.ToGalleryDto(0);
                });

            GalleryService
                .Setup(repo => repo.PutGalleryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GalleryPutDTO>()))
                .ReturnsAsync((Guid userId, Guid galleryId, GalleryPutDTO galleryPutDTO) =>
    {
        GalleryEntity galleryEntity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

        galleryPutDTO.ToGalleryEntity(ref galleryEntity);

        int numImagesInGallery = ImageEntities.FindAll(tmp => tmp.fk_gallery == galleryId).Count();

        return galleryEntity.ToGalleryDto(numImagesInGallery);
    });

            GalleryService
                .Setup(repo => repo.GetGalleryAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    GalleryEntity entity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);

                    int numImagesInGallery = ImageEntities.FindAll(tmp => tmp.fk_gallery == galleryId).Count();

                    return entity.ToGalleryDto(numImagesInGallery);
                });

            GalleryService
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

            GalleryService
                .Setup(repo => repo.DeleteGalleryAsync(It.IsAny<Guid>()))
                .Callback((Guid galleryId) =>
                {
                    GalleryEntity entity = GalleryEntities.FirstOrDefault(tmp => tmp.Id == galleryId);
                    GalleryEntities.Remove(entity);
                });
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
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            GalleryEntity newGallery = new GalleryEntity()
            {
                Id = Guid.NewGuid(),
                fk_owner = UserEntities[1].Id,
                Name = "TestName",
                owner = UserEntities[1]
            };
            GalleryEntities.Add(newGallery);

            // Act
            ActionResult<GalleryDTO> response = await controller.GetGallery(newGallery.Id);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(UnauthorizedResult));
            var result = response.Result as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
            GalleryEntities.Remove(newGallery);
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
            GalleryEntity newGallery = new GalleryEntity()
            {
                Id = Guid.NewGuid(),
                fk_owner = UserEntities[0].Id,
                Name = "TestName",
                owner = UserEntities[0]
            };
            GalleryEntities.Add(newGallery);

            // Act
            ActionResult response = await controller.DeleteGallery(newGallery.Id);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NoContentResult));
            var result = response as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);
            GalleryService.Verify(repo => repo.DeleteGalleryAsync(It.IsAny<Guid>()), Times.Once());
            GalleryEntities.Remove(newGallery);
        }

        [TestMethod]
        public async Task DeleteGallery_not_mine_and_it_exist()
        {
            // Arrange
            var controller = new GalleryController(GalleryService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());
            GalleryEntity newGallery = new GalleryEntity()
            {
                Id = Guid.NewGuid(),
                fk_owner = UserEntities[1].Id,
                Name = "TestName",
                owner = UserEntities[1]
            };
            GalleryEntities.Add(newGallery);

            // Act
            ActionResult response = await controller.DeleteGallery(newGallery.Id);

            // Assert
            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
            var result = response as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
            GalleryEntities.Remove(newGallery);
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
