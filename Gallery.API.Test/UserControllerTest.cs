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

namespace Gallery.API.Test
{
    [TestClass]
    public class UserControllerTest
    {
        private static Mock<IUserService> UserService;
        private static List<UserEntity> UserEntities = new List<UserEntity>();

        //[ClassInitialize]
        //public static void InitTestClass(TestContext testContext)
        [TestInitialize]
        public void InitBeforeEachTest()
        {
            // Init users entities
            UserEntities.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "Username1", Password = "Password1" });
            UserEntities.Add(new UserEntity() { Id = Guid.NewGuid(), Username = "Username2", Password = "Password2" });

            // Mock user service
            UserService = MockFactory.CreateUserServiceMock(UserEntities);
        }

        #region RequestTokenAsync
        [TestMethod]
        public async Task RequestTokenAsync_exist_correctPassword_get_token()
        {
            // Arrange
            var controller = new UserController(UserService.Object);
            UserLoginDTO userLoginDTO = new UserLoginDTO() {
                username = UserEntities.First().Username,
                password = UserEntities.First().Password
            };

            // Act
            IActionResult response = await controller.RequestTokenAsync(userLoginDTO);

            // Assert
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.IsInstanceOfType(result.Value, typeof(UserLoginResponseDTO));
            UserLoginResponseDTO loginDTO = result.Value as UserLoginResponseDTO;
            Assert.IsNotNull(loginDTO.token);
            Assert.IsInstanceOfType(loginDTO.token, typeof(string));
        }

        [TestMethod]
        public async Task RequestTokenAsync_exist_wrongPassword_return_unauthorized()
        {
            // Arrange
            var controller = new UserController(UserService.Object);
            UserLoginDTO userLoginDTO = new UserLoginDTO()
            {
                username = UserEntities.First().Username,
                password = "WrongPassword1"
            };

            // Act
            IActionResult response = await controller.RequestTokenAsync(userLoginDTO);

            // Assert
            Assert.IsInstanceOfType(response, typeof(UnauthorizedObjectResult));
            var result = response as UnauthorizedObjectResult;
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public async Task RequestTokenAsync_notExist_return_notFound()
        {
            // Arrange
            var controller = new UserController(UserService.Object);
            UserLoginDTO userLoginDTO = new UserLoginDTO()
            {
                username = "NotExistingUsername1",
                password = "NotExistingPassword1"
            };

            // Act
            IActionResult response = await controller.RequestTokenAsync(userLoginDTO);

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundObjectResult));
            var result = response as NotFoundObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }
        #endregion

        #region GetUser
        [TestMethod]
        public async Task GetUser_get_self()
        {
            // Arrange
            var controller = new UserController(UserService.Object);
            UserEntity retrieveThisUserItem = UserEntities[0];
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[0].Id.ToString());

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
            var controller = new UserController(UserService.Object);
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(UserEntities[1].Id.ToString());

            // Act
            ActionResult<UserDTO> response = await controller.GetUser(UserEntities[0].Id);

            // Assert
            Assert.IsInstanceOfType(response.Result, typeof(UnauthorizedResult));
            var result = response.Result as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public async Task GetUser_get_self_not_exist()
        {
            // Arrange
            var controller = new UserController(UserService.Object);
            var randomGuid = Guid.NewGuid();
            controller.ControllerContext = APIControllerUtils.CreateApiControllerContext(randomGuid.ToString());

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
            var controller = new UserController(UserService.Object);
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
            UserEntities.Add(existingUser);
            var controller = new UserController(UserService.Object);
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
            UserEntities.Remove(existingUser);
            //userRepository.Verify(repo => repo.GetUser(It.IsAny<string>()), Times.Once());
        }
        #endregion
    }
}
