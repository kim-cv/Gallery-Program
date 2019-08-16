using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery.API.Entities
{
    public class ImageEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid fk_gallery { get; set; }

        [ForeignKey("fk_gallery")]
        public GalleryEntity gallery { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Extension { get; set; }

        [Required]
        public uint SizeInBytes { get; set; }
    }
}
