using System;
using System.Threading.Tasks;
using Gallery.API.Entities;

namespace Gallery.API.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUser(Guid userId);
        UserEntity GetUser(string username);
        UserEntity GetUser(string username, string password);
        Task<UserEntity> PostUser(UserEntity userEntity);
        bool Save();
    }
}
