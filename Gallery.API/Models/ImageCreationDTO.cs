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
        public IFormFile FormFile { get; set; }

        public ImageCreationDTO(string name, IFormFile formFile)
        {
            Name = name;
            FormFile = formFile;
        }
    }
}
