using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Models
{
    public class UserCreationDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
