using Gallery.API.Entities;

namespace Gallery.API.Models
{
    public static class UserEntityToDtoMapper
    {
        public static UserDTO ToUserDto(this UserEntity userEntity)
        {
            return new UserDTO(userEntity.Id, userEntity.Username);
        }
    }
}
