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

namespace Gallery.API.Test
{
    [TestClass]
    public class UserControllerTest
    {
        private static Mock<IUserRepository> userRepository;
        private static List<UserEntity> users = new List<UserEntity>();

        [ClassInitialize]
        public static void InitTestClass(TestContext testContext)
        {
            users.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "Username1", Password = "Password1" });
            users.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "Username2", Password = "Password2" });

            userRepository = new Mock<IUserRepository>();

            userRepository.Setup(repo => repo.GetUser(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) =>
                {
                    return users.FirstOrDefault(tmpUser => tmpUser.Id == id);
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
        }

        #region GetUser
        [TestMethod]
        public async Task GetUser_exist()
        {
            // Arrange
            var controller = new UserController(userRepository.Object);
            var retrieveThisUserItem = users[0];

            // Act
            ActionResult<UserDTO> response = await controller.GetUser(retrieveThisUserItem.Id);
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(UserDTO));
            UserDTO retrievedItem = result.Value as UserDTO;
            Assert.AreEqual(retrieveThisUserItem.Id, retrievedItem.Id);
            Assert.AreEqual(retrieveThisUserItem.Username, retrievedItem.Username);
        }

        [TestMethod]
        public async Task GetUser_not_exist()
        {
            // Arrange
            var controller = new UserController(userRepository.Object);

            // Act
            ActionResult<UserDTO> response = await controller.GetUser(Guid.NewGuid());
            var result = response.Result as NotFoundResult;

            // Assert
            Assert.AreEqual(404, result.StatusCode);
        }
        #endregion

        #region CreateUser
        [TestMethod]
        public async Task CreateUser()
        {
            // Arrange
            var controller = new UserController(userRepository.Object);
            UserCreationDTO newUser = new UserCreationDTO()
            {
                Username = "CreatedTestUsername",
                Password = "CreatedTestPassword"
            };

            // Act
            ActionResult<UserDTO> response = await controller.CreateUser(newUser);
            var result = response.Result as CreatedAtActionResult;

            // Assert            
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(UserDTO));
            UserDTO createdItem = result.Value as UserDTO;
            Assert.AreEqual(createdItem.Username, "CreatedTestUsername");
        }
        #endregion
    }
}
