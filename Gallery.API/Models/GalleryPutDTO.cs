using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Models
{
    public class GalleryPutDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
