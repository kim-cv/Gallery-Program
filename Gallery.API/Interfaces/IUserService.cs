using System;
using System.Threading.Tasks;
using Gallery.API.Models;

namespace Gallery.API.Interfaces
{
    public interface IUserService
    {
        Task<bool> DoesUserExist(Guid userId);
        Task<bool> DoesUserExist(string username);
        Task<UserDTO> CreateUserAsync(UserCreationDTO dto);
        Task<UserDTO> GetUserAsync(Guid userId);
        UserDTO GetUserAsync(string username);

        Task<string> LoginAsync(UserLoginDTO userLoginDTO);
    }
}
