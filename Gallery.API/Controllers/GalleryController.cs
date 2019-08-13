using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public GalleryController(IGalleryRepository galleryRepository)
        {
            _galleryRepository = galleryRepository;
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
    }
}
