using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Gallery.API.Entities;
using Gallery.API.Interfaces;

namespace Gallery.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly GalleryDBContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(GalleryDBContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserEntity> GetUser(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public UserEntity GetUser(string username)
        {
            return _context.Users.FirstOrDefault(tmpUser => tmpUser.Username == username);
        }

        public UserEntity GetUser(string username, string password)
        {
            return _context.Users.FirstOrDefault(tmpUser => tmpUser.Username == username && tmpUser.Password == password);
        }

        public async Task<UserEntity> PostUser(UserEntity userEntity)
        {
            var changeTracking = await _context.Users.AddAsync(userEntity);
            return changeTracking.Entity;
        }

        public bool Save()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Uncaught exception during method Save().", ex);
                return false;
            }
        }
    }
}
