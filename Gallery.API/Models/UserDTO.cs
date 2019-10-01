using System;

namespace Gallery.API.Models
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }

        public UserDTO(Guid id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}
