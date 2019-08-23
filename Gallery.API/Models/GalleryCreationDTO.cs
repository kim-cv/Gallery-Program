using System;
using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Models
{
    public class GalleryCreationDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
