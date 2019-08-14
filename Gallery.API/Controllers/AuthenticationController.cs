using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Models;

namespace Gallery.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticateService _authService;

        public AuthenticationController(IAuthenticateService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult RequestToken(UserLoginDTO request)
        {
            UserEntity userEntity = _userRepository.GetUser(request.username);
            if (userEntity == null)
            {
                return NotFound("User not found");
            }

            string hashedPassword = _authService.HashPassword(request.password, userEntity.Salt);
            if (userEntity.Password != hashedPassword)
            {
                return Unauthorized("Wrong Password");
            }

            string token = _authService.GenerateTokenForUser(userEntity.Id);
            return Ok(new { token });
        }
    }
}