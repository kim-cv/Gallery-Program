using System;

namespace Gallery.API.Interfaces
{
    public interface IAuthenticateService
    {
        string GenerateTokenForUser(Guid userUid);
        string HashPassword(string password, byte[] salt);
        byte[] GenerateSalt();
    }
}
