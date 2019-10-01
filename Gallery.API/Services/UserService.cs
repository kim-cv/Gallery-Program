using System;
using System.Threading.Tasks;
using Gallery.API.Entities;
using Gallery.API.Models;
using Gallery.API.Interfaces;

namespace Gallery.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticateService _authService;

        public UserService(IUserRepository userRepository, IAuthenticateService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<bool> DoesUserExist(Guid userId)
        {
            UserEntity existingUser = await _userRepository.GetUser(userId);
            return (existingUser != null);
        }

        public async Task<bool> DoesUserExist(string username)
        {
            UserEntity existingUser = _userRepository.GetUser(username);
            return (existingUser != null);
        }

        public async Task<UserDTO> CreateUserAsync(UserCreationDTO userCreationDTO)
        {
            UserEntity entity = userCreationDTO.ToUserEntity();

            byte[] salt = _authService.GenerateSalt();
            string hashedPassword = _authService.HashPassword(entity.Password, salt);
            entity.Password = hashedPassword;
            entity.Salt = salt;

            UserEntity addedEntity = await _userRepository.PostUser(entity);

            if (_userRepository.Save() == false)
            {
                throw new Exception();
            }

            return addedEntity.ToUserDto();
        }

        public async Task<UserDTO> GetUserAsync(Guid userId)
        {
            UserEntity userEntity = await _userRepository.GetUser(userId);
            return userEntity.ToUserDto();
        }

        public UserDTO GetUserAsync(string username)
        {
            UserEntity userEntity = _userRepository.GetUser(username);
            return userEntity.ToUserDto();
        }


        public async Task<string> LoginAsync(UserLoginDTO userLoginDTO)
        {
            UserEntity userEntity = _userRepository.GetUser(userLoginDTO.Username);

            string hashedPassword = _authService.HashPassword(userLoginDTO.Password, userEntity.Salt);

            if (userEntity.Password != hashedPassword)
            {
                throw new Exception("Wrong Password");
            }

            return _authService.GenerateTokenForUser(userEntity.Id);
        }
    }
}
