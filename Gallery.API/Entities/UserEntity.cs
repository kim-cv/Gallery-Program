using System;
using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Entities
{
    public class UserEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public byte[] Salt { get; set; }
    }
}
