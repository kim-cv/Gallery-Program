using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gallery.API.Interfaces;
using Gallery.API.Models;
using Gallery.API.Helpers;

namespace Gallery.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/galleries")]
    public class GalleryController : ControllerBase
    {
        private readonly IGalleryService _galleryService;

        public GalleryController(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GalleryDTO>>> GetGalleries([FromQuery] Pagination pagination)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            IEnumerable<GalleryDTO> galleryDTOs = await _galleryService.GetGalleriesByUserAsync(userId, pagination);

            return Ok(galleryDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GalleryDTO>> GetGallery(Guid id)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            if (await _galleryService.DoesGalleryExistAsync(id) == false)
            {
                return NotFound();
            }

            if (await _galleryService.IsGalleryOwnedByUserAsync(id, userId) == false)
            {
                return Unauthorized();
            }

            GalleryDTO dto = await _galleryService.GetGalleryAsync(id);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<GalleryDTO>> CreateGallery(GalleryCreationDTO creationDto)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            try
            {
                GalleryDTO dto = await _galleryService.CreateGalleryAsync(userId, creationDto);

                return CreatedAtAction(nameof(GetGallery), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Unable to create the gallery at this moment due to an error, the error has been logged and sent to the developers for fixing.",
                    Instance = HttpContext.TraceIdentifier,
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpPut("{galleryId}")]
        public async Task<ActionResult<GalleryDTO>> PutGallery(Guid galleryId, GalleryPutDTO galleryPutDTO)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            if (await _galleryService.DoesGalleryExistAsync(galleryId) == false)
            {
                return NotFound();
            }

            if (await _galleryService.IsGalleryOwnedByUserAsync(galleryId, userId) == false)
            {
                return Unauthorized();
            }

            try
            {
                GalleryDTO galleryDto = await _galleryService.PutGalleryAsync(userId, galleryId, galleryPutDTO);

                return CreatedAtAction(nameof(GetGallery), new { id = galleryDto.Id }, galleryDto);
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Unable to update the gallery at this moment due to an error, the error has been logged and sent to the developers for fixing.",
                    Instance = HttpContext.TraceIdentifier,
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGallery(Guid id)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            if (await _galleryService.DoesGalleryExistAsync(id) == false)
            {
                return NotFound();
            }

            if (await _galleryService.IsGalleryOwnedByUserAsync(id, userId) == false)
            {
                return Unauthorized();
            }

            try
            {
                await _galleryService.DeleteGalleryAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Unable to delete the gallery at this moment due to an error, the error has been logged and sent to the developers for fixing.",
                    Instance = HttpContext.TraceIdentifier,
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}
