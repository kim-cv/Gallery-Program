using Gallery.API.Entities;
using Gallery.API.Interfaces;

namespace Gallery.API.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;

        public UserManagementService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool IsValidUser(string username, string password)
        {
            UserEntity user = _userRepository.GetUser(username, password);
            return user != null;
        }
    }
}
