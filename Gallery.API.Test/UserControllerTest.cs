using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

        //[ClassInitialize]
        //public static void InitTestClass(TestContext testContext)
        [TestInitialize]
        public void InitBeforeEachTest()
        {
            users.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "Username1", Password = "Password1" });
            users.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "Username2", Password = "Password2" });

            userRepository = new Mock<IUserRepository>();

            userRepository.Setup(repo => repo.GetUser(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) =>
                {
                    return users.FirstOrDefault(tmpUser => tmpUser.Id == id);
                });
            userRepository.Setup(repo => repo.GetUser(It.IsAny<string>()))
                .Returns((string username) =>
                {
                    return users.FirstOrDefault(tmpUser => tmpUser.Username == username);
                });
            userRepository.Setup(repo => repo.GetUser(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns((string username, string password) =>
                 {
                     return users.FirstOrDefault(tmpUser => tmpUser.Username == username && tmpUser.Password == password);
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
        public async Task GetUser_get_self()
        {
            // Arrange
            var controller = new UserController(userRepository.Object);
            UserEntity retrieveThisUserItem = users[0];
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
            ActionResult<UserDTO> response = await controller.GetUser(retrieveThisUserItem.Id);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            var result = response.Result as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(UserDTO));
            UserDTO retrievedItem = result.Value as UserDTO;
            Assert.AreEqual(retrieveThisUserItem.Id, retrievedItem.Id);
            Assert.AreEqual(retrieveThisUserItem.Username, retrievedItem.Username);
        }

        [TestMethod]
        public async Task GetUser_not_allow_get_another_users_data()
        {
            /**
             * Return 401 Unauthorized if trying to retrieve another users data
             * This unittest tries to access users[0] by setting users[1] as claim
             */

            // Arrange
            var controller = new UserController(userRepository.Object);
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

            // Act
            ActionResult<UserDTO> response = await controller.GetUser(users[0].Id);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(UnauthorizedResult));
            var result = response.Result as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public async Task GetUser_get_self_not_exist()
        {
            // Arrange
            var controller = new UserController(userRepository.Object);
            var randomGuid = Guid.NewGuid();
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, randomGuid.ToString())
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
            ActionResult<UserDTO> response = await controller.GetUser(randomGuid);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
            var result = response.Result as NotFoundResult;
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
                Username = "CreateUserUsername",
                Password = "CreateUserPassword"
            };

            // Act
            ActionResult<UserDTO> response = await controller.CreateUser(newUser);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(CreatedAtActionResult));
            var result = response.Result as CreatedAtActionResult;
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(UserDTO));
            UserDTO createdItem = result.Value as UserDTO;
            Assert.AreEqual(createdItem.Username, "CreateUserUsername");
        }

        [TestMethod]
        public async Task CreateUser_username_used()
        {
            // Arrange
            var existingUser = new UserEntity()
            {
                Id = Guid.NewGuid(),
                Username = "CreateUserUsernameUsed",
                Password = "Password"
            };
            users.Add(existingUser);
            var controller = new UserController(userRepository.Object);
            UserCreationDTO newUser = new UserCreationDTO()
            {
                Username = "CreateUserUsernameUsed",
                Password = "Password"
            };

            // Act
            ActionResult<UserDTO> response = await controller.CreateUser(newUser);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(ConflictResult));
            var result = response.Result as ConflictResult;
            Assert.AreEqual(409, result.StatusCode);
            users.Remove(existingUser);
            userRepository.Verify(repo => repo.GetUser(It.IsAny<string>()), Times.Once());
        }
        #endregion
    }
}
