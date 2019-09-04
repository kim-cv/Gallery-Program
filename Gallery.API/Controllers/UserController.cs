using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Gallery.API.Interfaces;
using Gallery.API.Models;

namespace Gallery.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> RequestTokenAsync(UserLoginDTO userLoginDTO)
        {
            if (await _userService.DoesUserExist(userLoginDTO.username) == false)
            {
                return NotFound("User not found");
            }

            try
            {
                string jwtToken = await _userService.LoginAsync(userLoginDTO);
                return Ok(new { token = jwtToken });
            }
            catch (Exception ex)
            {
                // TODO: Better exception handling
                if (ex.Message == "Wrong Password")
                {
                    return Unauthorized("Wrong Password");
                }
                else
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "An unexpected error occurred.",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = "Unable to login at this moment due to an error, the error has been logged and sent to the developers for fixing.",
                        Instance = HttpContext.TraceIdentifier,
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            // Only allow user to request info about itself
            if (id != userId)
            {
                return Unauthorized();
            }

            if (await _userService.DoesUserExist(userId) == false)
            {
                return NotFound();
            }

            UserDTO userDTO = await _userService.GetUserAsync(id);

            return Ok(userDTO);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> CreateUser(UserCreationDTO dto)
        {
            // Check username not already used
            if (await _userService.DoesUserExist(dto.Username) == true)
            {
                return Conflict();
            }

            try
            {
                UserDTO userDTO = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetUser), new { id = userDTO.Id }, userDTO);
            }
            catch (Exception ex)
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
        }
    }
}