using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Models;

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

        public GalleryController(IHostingEnvironment environment, IGalleryRepository galleryRepository, IImageRepository imageRepository, IFileSystemRepository fileSystemRepository)
        {
            _galleryRepository = galleryRepository;
            _imageRepository = imageRepository;
            _environment = environment;
            _fileSystemRepository = fileSystemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GalleryDTO>>> GetGalleries()
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            var items = await _galleryRepository.GetGalleriesFromOwner(userId);

            IEnumerable<GalleryDTO> dtos = items.Select(tmpEntity => tmpEntity.ToGalleryDto());

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

            GalleryDTO dto = item.ToGalleryDto();

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<GalleryDTO>> CreateGallery(GalleryCreationDTO dto)
        {
            Guid userId = new Guid(HttpContext.User.Identity.Name);

            if (userId != dto.ownerId)
            {
                return BadRequest();
            }

            GalleryEntity entity = dto.ToGalleryEntity();

            GalleryEntity addedEntity = await _galleryRepository.PostGallery(entity);
            _galleryRepository.Save();

            GalleryDTO dtoToReturn = addedEntity.ToGalleryDto();

            return CreatedAtAction(nameof(GetGallery), new { id = dtoToReturn.Id }, dtoToReturn);
        }

        [HttpGet("{id}")]
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
        public async Task<ActionResult<IEnumerable<ImageDTO>>> GetImages(Guid galleryId)
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

            var items = await _imageRepository.GetImages(galleryId);

            IEnumerable<ImageDTO> dtos = items.Select(tmpEntity => tmpEntity.ToImageDto());

            return Ok(dtos);
        }

        [HttpGet("{galleryId}/images/{imageId}")]
        public async Task<ActionResult> GetImage(Guid galleryId, Guid imageId)
        {
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
                return Unauthorized();
            }

            await _imageRepository.DeleteImage(imageEntity.Id);
            _imageRepository.Save();

            return NoContent();
        }
    }
}
