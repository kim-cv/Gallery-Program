using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Models
{
    public class UserLoginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public UserLoginDTO(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
