using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery.API.Entities
{
    public class GalleryEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid fk_owner { get; set; }

        [ForeignKey("fk_owner")]
        public UserEntity owner { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
