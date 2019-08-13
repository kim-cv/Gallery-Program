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
            UserEntity userEntity = _userRepository.GetUser(request.username, request.password);
            if (userEntity == null)
            {
                return NotFound("User not found");
            }

            string token = _authService.GenerateTokenForUser(userEntity.Id);
            return Ok(new { token });
        }
    }
}