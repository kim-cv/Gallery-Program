using System;
using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Models
{
    public class UserLoginResponseDTO
    {
        [Required]
        public string Token { get; set; }

        public UserLoginResponseDTO(string token)
        {
            Token = token;
        }
    }
}
