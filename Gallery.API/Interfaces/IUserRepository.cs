using System;
using System.Threading.Tasks;
using Gallery.API.Entities;

namespace Gallery.API.Interfaces
{
    public interface IUserRepository
    {
        UserEntity GetUser(string username, string password);
        Task<UserEntity> GetUser(Guid userId);
        Task<UserEntity> PostUser(UserEntity userEntity);
        bool Save();
    }
}
