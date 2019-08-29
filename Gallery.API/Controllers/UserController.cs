using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Models;

namespace Gallery.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticateService _authService;

        public UserController(IAuthenticateService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            if (id != userId)
            {
                return Unauthorized();
            }

            UserEntity item = await _userRepository.GetUser(id);

            if (item == null)
            {
                return NotFound();
            }

            UserDTO dto = item.ToUserDto();

            return Ok(dto);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> CreateUser(UserCreationDTO dto)
        {
            // Check username not already used
            UserEntity existingUser = _userRepository.GetUser(dto.Username);
            if (existingUser != null)
            {
                return Conflict();
            }

            UserEntity entity = dto.ToUserEntity();

            byte[] salt = _authService.GenerateSalt();
            string hashedPassword = _authService.HashPassword(entity.Password, salt);
            entity.Password = hashedPassword;
            entity.Salt = salt;

            UserEntity addedEntity = await _userRepository.PostUser(entity);

            if (_userRepository.Save() == false)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Unable to create the user at this moment due to an error, the error has been logged and sent to the developers for fixing.",
                    Instance = HttpContext.TraceIdentifier,
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
            else
            {
                UserDTO dtoToReturn = addedEntity.ToUserDto();
                return CreatedAtAction(nameof(GetUser), new { id = dtoToReturn.Id }, dtoToReturn);
            }
        }
    }
}