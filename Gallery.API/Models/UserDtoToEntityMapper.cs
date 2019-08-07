using Gallery.API.Entities;

namespace Gallery.API.Models
{
    public static class UserDtoToEntityMapper
    {
        public static UserEntity ToUserEntity(this UserCreationDTO userDto)
        {
            return new UserEntity()
            {
                Username = userDto.Username,
                Password = userDto.Password
            };
        }
    }
}
