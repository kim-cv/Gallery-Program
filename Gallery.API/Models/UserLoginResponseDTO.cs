using System;
using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Models
{
    public class UserLoginResponseDTO
    {
        [Required]
        public string token { get; set; }
    }
}
