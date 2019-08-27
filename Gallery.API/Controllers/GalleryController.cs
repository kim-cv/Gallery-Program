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
using Microsoft.AspNetCore.Hosting;
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
        private readonly IGalleryRepository _galleryRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IHostingEnvironment _environment;
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly IImageService _imageService;

        public GalleryController(IHostingEnvironment environment, IGalleryRepository galleryRepository, IImageRepository imageRepository, IFileSystemRepository fileSystemRepository, IImageService imageService)
        {
            _galleryRepository = galleryRepository;
            _imageRepository = imageRepository;
            _environment = environment;
            _fileSystemRepository = fileSystemRepository;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GalleryDTO>>> GetGalleries([FromQuery] Pagination pagination)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            var items = await _galleryRepository.GetGalleriesFromOwner(userId, pagination);

            IEnumerable<GalleryDTO> dtos = items.Select(tmpEntity =>
            {
                int numImagesInGallery = _imageRepository.GetNumberOfImagesInGallery(tmpEntity.Id);
                return tmpEntity.ToGalleryDto(numImagesInGallery);
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GalleryDTO>> GetGallery(Guid id)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);
            GalleryEntity item = await _galleryRepository.GetGallery(id);

            if (item == null)
            {
                return NotFound();
            }

            if (userId != item.fk_owner)
            {
                return Unauthorized();
            }

            int numImagesInGallery = _imageRepository.GetNumberOfImagesInGallery(item.Id);
            GalleryDTO dto = item.ToGalleryDto(numImagesInGallery);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<GalleryDTO>> CreateGallery(GalleryCreationDTO dto)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            GalleryEntity entity = dto.ToGalleryEntity(userId);

            GalleryEntity addedEntity = await _galleryRepository.PostGallery(entity);
            _galleryRepository.Save();

            GalleryDTO dtoToReturn = addedEntity.ToGalleryDto(0);

            return CreatedAtAction(nameof(GetGallery), new { id = dtoToReturn.Id }, dtoToReturn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GalleryDTO>> PutGallery(Guid id, GalleryPutDTO galleryPutDTO)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);
            GalleryEntity galleryEntity = await _galleryRepository.GetGallery(id);

            if (galleryEntity == null)
            {
                return NotFound();
            }

            if (userId != galleryEntity.fk_owner)
            {
                return Unauthorized();
            }

            galleryPutDTO.ToGalleryEntity(ref galleryEntity);

            await _galleryRepository.PutGallery(galleryEntity);
            _galleryRepository.Save();

            int numImagesInGallery = _imageRepository.GetNumberOfImagesInGallery(galleryEntity.Id);
            GalleryDTO dtoToReturn = galleryEntity.ToGalleryDto(numImagesInGallery);

            return CreatedAtAction(nameof(GetGallery), new { id = dtoToReturn.Id }, dtoToReturn);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGallery(Guid id)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);
            GalleryEntity item = await _galleryRepository.GetGallery(id);

            if (item == null)
            {
                return NotFound();
            }

            if (userId != item.fk_owner)
            {
                return Unauthorized();
            }

            await _galleryRepository.DeleteGallery(item.Id);
            _galleryRepository.Save();

            return NoContent();
        }


        [HttpGet("{galleryId}/images")]
        public async Task<ActionResult<IEnumerable<ImageDTO>>> GetImages(Guid galleryId, [FromQuery] Pagination pagination, [FromQuery] bool thumb = false)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            var gallery = await _galleryRepository.GetGallery(galleryId);

            if (gallery == null)
            {
                return NotFound();
            }

            if (gallery.fk_owner != userId)
            {
                return Unauthorized();
            }

            var items = await _imageRepository.GetImages(galleryId, pagination);

            IEnumerable<ImageDTO> dtos = items.Select(tmpEntity => tmpEntity.ToImageDto());

            return Ok(dtos);
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
            GalleryEntity gallery = await _galleryRepository.GetGallery(galleryId);
            ImageEntity image = await _imageRepository.GetImage(imageId);

            if (gallery == null || image == null)
            {
                return NotFound();
            }

            if (gallery.Id != image.fk_gallery)
            {
                return NotFound();
            }

            if (userId != gallery.fk_owner)
            {
                return Unauthorized();
            }

            var uploads = Path.Combine(_environment.ContentRootPath, "uploads");
            byte[] imgData = await _fileSystemRepository.RetrieveFile(uploads, image.Id.ToString(), image.Extension);

            if (thumb == true)
            {
                imgData = _imageService.GenerateThumb(imgData, (int)thumbWidth, (int)thumbHeight, (bool)keepAspectRatio);
            }

            return File(imgData, "image/jpeg");
        }

        [Consumes("multipart/form-data")]
        [HttpPost("{galleryId}/images")]
        public async Task<ActionResult> CreateImage(Guid galleryId, [FromForm] ImageCreationDTO dto)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            GalleryEntity gallery = await _galleryRepository.GetGallery(galleryId);

            if (gallery == null)
            {
                return NotFound();
            }

            if (userId != gallery.fk_owner)
            {
                return Unauthorized();
            }

            ImageEntity entity = dto.ToImageEntity();
            entity.fk_gallery = gallery.Id;

            ImageEntity addedEntity = await _imageRepository.PostImage(entity);
            _imageRepository.Save();

            var uploads = Path.Combine(_environment.ContentRootPath, "uploads");
            IFormFile formFile = dto.formFile;
            if (formFile.Length > 0)
            {
                string extension = Path.GetExtension(formFile.FileName);
                string filename = addedEntity.Id.ToString();

                byte[] formfileBytes;
                using (Stream stream = formFile.OpenReadStream())
                {
                    formfileBytes = new byte[stream.Length];
                    await stream.ReadAsync(formfileBytes, 0, (int)stream.Length);
                }

                await _fileSystemRepository.SaveFile(uploads, formfileBytes, filename, extension);
            }

            ImageDTO dtoToReturn = addedEntity.ToImageDto();

            return CreatedAtAction(nameof(GetImage), new { galleryId = gallery.Id, imageId = dtoToReturn.Id }, dtoToReturn);
        }

        [HttpDelete("{galleryId}/images/{imageId}")]
        public async Task<ActionResult> DeleteImage(Guid galleryId, Guid imageId)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);
            GalleryEntity galleryEntity = await _galleryRepository.GetGallery(galleryId);
            ImageEntity imageEntity = await _imageRepository.GetImage(imageId);

            if (galleryEntity == null)
            {
                return NotFound();
            }

            if (imageEntity == null)
            {
                return NotFound();
            }

            if (userId != galleryEntity.fk_owner)
            {
                return Unauthorized();
            }

            if (imageEntity.fk_gallery != galleryEntity.Id)
            {
                return NotFound();
            }

            // Delete from filesystem
            var uploads = Path.Combine(_environment.ContentRootPath, "uploads");
            _fileSystemRepository.DeleteFile(uploads, imageEntity.Id.ToString(), imageEntity.Extension);

            // Delete from DB
            await _imageRepository.DeleteImage(imageEntity.Id);
            _imageRepository.Save();

            return NoContent();
        }
    }
}
