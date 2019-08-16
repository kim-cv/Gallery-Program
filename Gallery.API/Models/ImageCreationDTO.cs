using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Gallery.API.Models
{
    public class ImageCreationDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public IFormFile formFile { get; set; }
    }
}
