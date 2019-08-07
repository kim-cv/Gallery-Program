using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Models
{
    public class UserLoginDTO
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}
