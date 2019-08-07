﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Gallery.API.Entities;
using Gallery.API.Interfaces;

namespace Gallery.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public UserEntity GetUser(string username, string password)
        {
            return _context.Users.FirstOrDefault(tmpUser => tmpUser.Username == username && tmpUser.Password == password);
        }

        public async Task<UserEntity> GetUser(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
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
                return false;
            }
        }
    }
}
