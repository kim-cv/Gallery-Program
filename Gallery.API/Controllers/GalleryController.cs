using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Gallery.API.Entities;
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
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly IImageService _imageService;

        public GalleryController(IGalleryService galleryService, IFileSystemRepository fileSystemRepository, IImageService imageService)
        {
            _galleryService = galleryService;
            _fileSystemRepository = fileSystemRepository;
            _imageService = imageService;
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


        [HttpGet("{galleryId}/images")]
        public async Task<ActionResult<IEnumerable<byte[]>>> GetImages(
            Guid galleryId,
            [FromQuery] Pagination pagination,
            [FromQuery, BindRequired] bool thumb,
            [FromQuery, Range(1, 4096)] int? thumbWidth,
            [FromQuery, Range(1, 2160)] int? thumbHeight,
            [FromQuery] bool? keepAspectRatio)
        {
            // TODO: Very bad conditional validation, when fix this maybe take a look at fluent validation.
            if (thumb)
            {
                IList<string> conditionalValidationErrors = new List<string>();
                if (thumbWidth == null) conditionalValidationErrors.Add("You set 'thumb' to true, please also provide 'thumbWidth'");
                if (thumbHeight == null) conditionalValidationErrors.Add("You set 'thumb' to true, please also provide 'thumbHeight'");
                if (keepAspectRatio == null) conditionalValidationErrors.Add("You set 'thumb' to true, please also provide 'keepAspectRatio'");

                if (conditionalValidationErrors.Count > 0)
                {
                    return BadRequest(conditionalValidationErrors);
                }
            }

            Guid userId = new Guid(HttpContext.User.Identity.Name);

            if (await _galleryService.DoesGalleryExistAsync(galleryId) == false)
            {
                return NotFound();
            }

            if (await _galleryService.IsGalleryOwnedByUserAsync(galleryId, userId) == false)
            {
                return Unauthorized();
            }

            IEnumerable<byte[]> images = await _imageService.GetImagesInGalleryAsync(galleryId, pagination, thumb, thumbWidth, thumbHeight, keepAspectRatio);

            return Ok(images);
        }

        [HttpGet("{galleryId}/images/{imageId}")]
        public async Task<ActionResult> GetImage(
            Guid galleryId,
            Guid imageId,
            [FromQuery, BindRequired] bool thumb,
            [FromQuery, Range(1, 4096)] int? thumbWidth,
            [FromQuery, Range(1, 2160)] int? thumbHeight,
            [FromQuery] bool? keepAspectRatio
            )
        {
            // TODO: Very bad conditional validation, when fix this maybe take a look at fluent validation.
            if (thumb)
            {
                IList<string> conditionalValidationErrors = new List<string>();
                if (thumbWidth == null) conditionalValidationErrors.Add("You set 'thumb' to true, please also provide 'thumbWidth'");
                if (thumbHeight == null) conditionalValidationErrors.Add("You set 'thumb' to true, please also provide 'thumbHeight'");
                if (keepAspectRatio == null) conditionalValidationErrors.Add("You set 'thumb' to true, please also provide 'keepAspectRatio'");

                if (conditionalValidationErrors.Count > 0)
                {
                    return BadRequest(conditionalValidationErrors);
                }
            }

            Guid userId = new Guid(HttpContext.User.Identity.Name);

            if (await _galleryService.DoesGalleryExistAsync(galleryId) == false)
            {
                return NotFound();
            }

            if (await _imageService.DoesImageExistAsync(imageId) == false)
            {
                return NotFound();
            }

            if (await _imageService.IsImageInsideGalleryAsync(imageId, galleryId) == false)
            {
                return NotFound();
            }

            if (await _galleryService.IsGalleryOwnedByUserAsync(galleryId, userId) == false)
            {
                return Unauthorized();
            }

            byte[] image = await _imageService.GetImageAsync(imageId, thumb, thumbWidth, thumbHeight, keepAspectRatio);

            return File(image, "image/jpeg");
        }

        [Consumes("multipart/form-data")]
        [HttpPost("{galleryId}/images")]
        public async Task<ActionResult> CreateImage(Guid galleryId, [FromForm] ImageCreationDTO dto)
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
                ImageDTO imageDTO = await _imageService.CreateImageAsync(userId, galleryId, dto);

                return CreatedAtAction(nameof(GetImage), new { galleryId = galleryId, imageId = imageDTO.Id }, imageDTO);
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Unable to create the image at this moment due to an error, the error has been logged and sent to the developers for fixing.",
                    Instance = HttpContext.TraceIdentifier,
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpDelete("{galleryId}/images/{imageId}")]
        public async Task<ActionResult> DeleteImage(Guid galleryId, Guid imageId)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            if (await _galleryService.DoesGalleryExistAsync(galleryId) == false)
            {
                return NotFound();
            }

            if (await _imageService.DoesImageExistAsync(imageId) == false)
            {
                return NotFound();
            }

            if (await _galleryService.IsGalleryOwnedByUserAsync(galleryId, userId) == false)
            {
                return Unauthorized();
            }

            if (await _imageService.IsImageInsideGalleryAsync(imageId, galleryId) == false)
            {
                return NotFound();
            }

            try
            {
                await _imageService.DeleteImageAsync(galleryId, imageId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Unable to delete the image at this moment due to an error, the error has been logged and sent to the developers for fixing.",
                    Instance = HttpContext.TraceIdentifier,
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}
