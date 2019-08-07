using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Gallery.API.Controllers;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Models;

namespace Gallery.API.Test
{
    [TestClass]
    public class GalleryControllerTest
    {
        private static Mock<IUserRepository> userRepository;
        private static List<UserEntity> users = new List<UserEntity>();

        private static Mock<IGalleryRepository> galleryRepository;
        private static List<GalleryEntity> galleryItems = new List<GalleryEntity>();

        [ClassInitialize]
        public static void InitTestClass(TestContext testContext)
        {
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

            galleryRepository = new Mock<IGalleryRepository>();
            galleryRepository.Setup(repo => repo.GetGallery(It.IsAny<Guid>()))
                .ReturnsAsync((Guid galleryId) =>
                {
                    return galleryItems.FirstOrDefault(tmpGallery => tmpGallery.Id == galleryId);
                });
            galleryRepository.Setup(repo => repo.GetGalleries())
                .ReturnsAsync(() =>
                {
                    return galleryItems;
                });
            galleryRepository.Setup(repo => repo.PostGallery(It.IsAny<GalleryEntity>()))
                .ReturnsAsync((GalleryEntity galleryEntity) =>
                {
                    galleryItems.Add(galleryEntity);
                    return galleryEntity;
                });
            galleryRepository.Setup(repo => repo.Save())
                .Returns(() =>
                {
                    return true;
                });

            galleryItems.Add(new GalleryEntity() { Id = Guid.NewGuid(), fk_owner = users[0].Id, Name = "TestName1" });
            galleryItems.Add(new GalleryEntity() { Id = Guid.NewGuid(), fk_owner = users[0].Id, Name = "TestName2" });
        }

        #region GetGallery
        [TestMethod]
        public async Task GetGallery_exist()
        {
            // Arrange
            var controller = new GalleryController(galleryRepository.Object);
            var retrieveThisGalleryItem = galleryItems[0];

            // Act
            ActionResult<GalleryDTO> response = await controller.GetGallery(retrieveThisGalleryItem.Id);
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(GalleryDTO));
            GalleryDTO retrievedItem = result.Value as GalleryDTO;
            Assert.AreEqual(retrieveThisGalleryItem.Id, retrievedItem.Id);
            Assert.AreEqual(retrieveThisGalleryItem.Name, retrievedItem.Name);
        }

        [TestMethod]
        public async Task GetGallery_not_exist()
        {
            // Arrange
            var controller = new GalleryController(galleryRepository.Object);

            // Act
            ActionResult<GalleryDTO> response = await controller.GetGallery(Guid.NewGuid());
            var result = response.Result as NotFoundResult;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
        }
        #endregion

        #region GetGalleries
        [TestMethod]
        public async Task GetGalleries()
        {
            // Arrange
            var controller = new GalleryController(galleryRepository.Object);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, users[0].Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            controller.ControllerContext = context;

            // Act
            ActionResult<IEnumerable<GalleryDTO>> response = await controller.GetGalleries();
            var result = response.Result as OkObjectResult;

            // Assert
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
            var controller = new GalleryController(galleryRepository.Object);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, users[1].Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            controller.ControllerContext = context;

            GalleryCreationDTO newGalleryItem = new GalleryCreationDTO()
            {
                Name = "CreatedTestName",
                ownerId = users[1].Id
            };

            // Act
            ActionResult<GalleryDTO> response = await controller.CreateGallery(newGalleryItem);
            var result = response.Result as CreatedAtActionResult;

            // Assert            
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(GalleryDTO));
            GalleryDTO createdItem = result.Value as GalleryDTO;
            Assert.AreEqual(createdItem.Name, "CreatedTestName");
        }

        [TestMethod]
        public async Task CreateGallery_incorrect_ownerId()
        {
            // Arrange
            var controller = new GalleryController(galleryRepository.Object);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, users[1].Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            controller.ControllerContext = context;

            GalleryCreationDTO newGalleryItem = new GalleryCreationDTO()
            {
                Name = "CreatedTestName",
                ownerId = Guid.NewGuid()
            };

            // Act
            ActionResult<GalleryDTO> response = await controller.CreateGallery(newGalleryItem);
            var result = response.Result as BadRequestResult;

            // Assert            
            Assert.AreEqual(400, result.StatusCode);
        }
        #endregion
    }
}
