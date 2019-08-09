using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(IUserRepository userRepository)
        {
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
            UserEntity entity = dto.ToUserEntity();

            UserEntity addedEntity = await _userRepository.PostUser(entity);
            _userRepository.Save();

            UserDTO dtoToReturn = addedEntity.ToUserDto();

            return CreatedAtAction(nameof(GetUser), new { id = dtoToReturn.Id }, dtoToReturn);
        }
    }
}